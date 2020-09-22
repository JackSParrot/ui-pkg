using System;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.UI
{
    public class VerticalRecyclingSystem : RecyclingSystem
    {
        public override void Initialize(RectTransform prototypeCell, RectTransform viewport, RectTransform content, IScrollListDataSource dataSource, bool isGrid, int lines)
        {
            base.Initialize(prototypeCell, viewport, content, dataSource, isGrid, lines);
            SetTopAnchor(Content);

            Content.anchoredPosition = Vector3.zero;
            SetRecyclingBounds();

            CreateCellPool();
            CurrentItemCount = CellPool.Count;
            HighestCellIndex = 0;
            LowestCellIndex = CellPool.Count - 1;

            int noOfRows = Mathf.CeilToInt(CellPool.Count / (float)Lines);
            float contentYSize = noOfRows * CellHeight;
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, contentYSize);

            SetTopAnchor(Content);
        }

        private void SetRecyclingBounds()
        {
            Viewport.GetWorldCorners(Corners);
            float threshHold = RecyclingThreshold * (Corners[2].y - Corners[0].y);
            RecyclableViewBounds.min = new Vector3(Corners[0].x, Corners[0].y - threshHold);
            RecyclableViewBounds.max = new Vector3(Corners[2].x, Corners[2].y + threshHold);
        }

        private void CreateCellPool()
        {
            if (CellPool != null)
            {
                CellPool.ForEach((RectTransform item) => UnityEngine.Object.Destroy(item.gameObject));
                CellPool.Clear();
                CachedCells.Clear();
            }
            else
            {
                CachedCells = new List<Transform>();
                CellPool = new List<RectTransform>();
            }

            PrototypeCell.gameObject.SetActive(true);
            if (IsGrid)
            {
                SetTopLeftAnchor(PrototypeCell);
            }
            else
            {
                SetTopAnchor(PrototypeCell);
            }

            float currentPoolCoverage = 0;
            int poolSize = 0;
            float posX = 0;
            float posY = 0;

            CellWidth = Content.rect.width / Lines;
            CellHeight = PrototypeCell.sizeDelta.y / PrototypeCell.sizeDelta.x * CellWidth;

            float requriedCoverage = MinPoolCoverage * Viewport.rect.height;
            int minPoolSize = Math.Min(MinPoolSize, DataSource.GetItemCount());

            while ((poolSize < minPoolSize || currentPoolCoverage < requriedCoverage) && poolSize < DataSource.GetItemCount())
            {
                RectTransform item = UnityEngine.Object.Instantiate(PrototypeCell.gameObject).GetComponent<RectTransform>();
                item.name = "Cell";
                item.sizeDelta = new Vector2(CellWidth, CellHeight);
                CellPool.Add(item);
                item.SetParent(Content, false);

                if (IsGrid)
                {
                    posX = LowestCellLine * CellWidth;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++LowestCellLine >= Lines)
                    {
                        LowestCellLine = 0;
                        posY -= CellHeight;
                        currentPoolCoverage += item.rect.height;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(0, posY);
                    posY = item.anchoredPosition.y - item.rect.height;
                    currentPoolCoverage += item.rect.height;
                }

                CachedCells.Add(item.GetComponent<Transform>());
                DataSource.SetCell(CachedCells[CachedCells.Count - 1], poolSize);

                poolSize++;
            }

            if (IsGrid)
            {
                LowestCellLine = (LowestCellLine - 1 + Lines) % Lines;
            }

            if (PrototypeCell.gameObject.scene.IsValid())
            {
                PrototypeCell.gameObject.SetActive(false);
            }
        }

        public override Vector2 OnValueChangedListener(Vector2 direction)
        {
            if (Recycling || CellPool == null || CellPool.Count == 0) return ZeroVector;

            SetRecyclingBounds();

            if (direction.y > 0 && CellPool[LowestCellIndex].MaxY() > RecyclableViewBounds.min.y)
            {
                return RecycleTopToBottom();
            }
            else if (direction.y < 0 && CellPool[HighestCellIndex].MinY() < RecyclableViewBounds.max.y)
            {
                return RecycleBottomToTop();
            }

            return ZeroVector;
        }

        private Vector2 RecycleTopToBottom()
        {
            Recycling = true;

            int n = 0;
            float posY = IsGrid ? CellPool[LowestCellIndex].anchoredPosition.y : 0;
            float posX = 0;

            int additionalRows = 0;
            while (CellPool[HighestCellIndex].MinY() > RecyclableViewBounds.max.y && CurrentItemCount < DataSource.GetItemCount())
            {
                if (IsGrid)
                {
                    if (++LowestCellLine >= Lines)
                    {
                        n++;
                        LowestCellLine = 0;
                        posY = CellPool[LowestCellIndex].anchoredPosition.y - CellHeight;
                        additionalRows++;
                    }
                    posX = LowestCellLine * CellWidth;
                    CellPool[HighestCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++HighestCellLine >= Lines)
                    {
                        HighestCellLine = 0;
                        additionalRows--;
                    }
                }
                else
                {
                    posY = CellPool[LowestCellIndex].anchoredPosition.y - CellPool[LowestCellIndex].sizeDelta.y;
                    CellPool[HighestCellIndex].anchoredPosition = new Vector2(CellPool[HighestCellIndex].anchoredPosition.x, posY);
                }

                DataSource.SetCell(CachedCells[HighestCellIndex], CurrentItemCount);

                LowestCellIndex = HighestCellIndex;
                HighestCellIndex = (HighestCellIndex + 1) % CellPool.Count;

                CurrentItemCount++;
                if (!IsGrid) n++;
            }
            if (IsGrid)
            {
                Content.sizeDelta += additionalRows * Vector2.up * CellHeight;
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            CellPool.ForEach((RectTransform cell) => cell.anchoredPosition += n * Vector2.up * CellPool[HighestCellIndex].sizeDelta.y);
            Content.anchoredPosition -= n * Vector2.up * CellPool[HighestCellIndex].sizeDelta.y;
            Recycling = false;
            return -new Vector2(0, n * CellPool[HighestCellIndex].sizeDelta.y);

        }

        private Vector2 RecycleBottomToTop()
        {
            Recycling = true;

            int n = 0;
            float posY = IsGrid ? CellPool[HighestCellIndex].anchoredPosition.y : 0;
            float posX = 0;

            int additionalRows = 0;
            while (CellPool[LowestCellIndex].MaxY() < RecyclableViewBounds.min.y && CurrentItemCount > CellPool.Count)
            {

                if (IsGrid)
                {
                    if (--HighestCellLine < 0)
                    {
                        n++;
                        HighestCellLine = Lines - 1;
                        posY = CellPool[HighestCellIndex].anchoredPosition.y + CellHeight;
                        additionalRows++;
                    }
                    posX = HighestCellLine * CellWidth;
                    CellPool[LowestCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--LowestCellLine < 0)
                    {
                        LowestCellLine = Lines - 1;
                        additionalRows--;
                    }
                }
                else
                {
                    posY = CellPool[HighestCellIndex].anchoredPosition.y + CellPool[HighestCellIndex].sizeDelta.y;
                    CellPool[LowestCellIndex].anchoredPosition = new Vector2(CellPool[LowestCellIndex].anchoredPosition.x, posY);
                    n++;
                }

                CurrentItemCount--;
                DataSource.SetCell(CachedCells[LowestCellIndex], CurrentItemCount - CellPool.Count);
                HighestCellIndex = LowestCellIndex;
                LowestCellIndex = (LowestCellIndex - 1 + CellPool.Count) % CellPool.Count;
            }

            if (IsGrid)
            {
                Content.sizeDelta += additionalRows * Vector2.up * CellHeight;
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            CellPool.ForEach((RectTransform cell) => cell.anchoredPosition -= n * Vector2.up * CellPool[HighestCellIndex].sizeDelta.y);
            Content.anchoredPosition += n * Vector2.up * CellPool[HighestCellIndex].sizeDelta.y;
            Recycling = false;
            return new Vector2(0, n * CellPool[HighestCellIndex].sizeDelta.y);
        }

        private void SetTopAnchor(RectTransform rectTransform)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);

            rectTransform.sizeDelta = new Vector2(width, height);
        }

        private void SetTopLeftAnchor(RectTransform rectTransform)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RecyclableViewBounds.min - new Vector3(2000, 0), RecyclableViewBounds.min + new Vector3(2000, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(RecyclableViewBounds.max - new Vector3(2000, 0), RecyclableViewBounds.max + new Vector3(2000, 0));
        }
    }
}