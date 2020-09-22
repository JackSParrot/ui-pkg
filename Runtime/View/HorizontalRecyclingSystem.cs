using System;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.UI
{
    public class HorizontalRecyclingSystem : RecyclingSystem
    {
        public override void Initialize(RectTransform prototypeCell, RectTransform viewport, RectTransform content, IScrollListDataSource dataSource, bool isGrid, int lines)
        {
            base.Initialize(prototypeCell, viewport, content, dataSource, isGrid, lines);
            SetLeftAnchor(Content);

            Content.anchoredPosition = Vector3.zero;
            SetRecyclingBounds();

            CreateCellPool();
            CurrentItemCount = CellPool.Count;
            LowestCellIndex = 0;
            HighestCellIndex = CellPool.Count - 1;

            int coloums = Mathf.CeilToInt(CellPool.Count / (float)Lines);
            float contentXSize = coloums * CellWidth;
            Content.sizeDelta = new Vector2(contentXSize, Content.sizeDelta.y);

            SetLeftAnchor(Content);
        }

        private void SetRecyclingBounds()
        {
            Viewport.GetWorldCorners(Corners);
            float threshHold = RecyclingThreshold * (Corners[2].x - Corners[0].x);
            RecyclableViewBounds.min = new Vector3(Corners[0].x - threshHold, Corners[0].y);
            RecyclableViewBounds.max = new Vector3(Corners[2].x + threshHold, Corners[2].y);
        }

        private void CreateCellPool()
        {
            if (CellPool != null)
            {
                CellPool.ForEach(i => UnityEngine.Object.Destroy(i.gameObject));
                CellPool.Clear();
                CachedCells.Clear();
            }
            else
            {
                CachedCells = new List<Transform>();
                CellPool = new List<RectTransform>();
            }

            PrototypeCell.gameObject.SetActive(true);
            SetLeftAnchor(PrototypeCell);

            float currentPoolCoverage = 0;
            int poolSize = 0;
            float posX = 0;
            float posY = 0;

            CellHeight = Content.rect.height / Lines;
            CellWidth = PrototypeCell.sizeDelta.x / PrototypeCell.sizeDelta.y * CellHeight;

            float requriedCoverage = MinPoolCoverage * Viewport.rect.width;
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
                    posY = -HighestCellLine * CellHeight;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++HighestCellLine >= Lines)
                    {
                        HighestCellLine = 0;
                        posX += CellWidth;
                        currentPoolCoverage += item.rect.width;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(posX, 0);
                    posX = item.anchoredPosition.x + item.rect.width;
                    currentPoolCoverage += item.rect.width;
                }

                CachedCells.Add(item.GetComponent<Transform>());
                DataSource.SetCell(CachedCells[CachedCells.Count - 1], poolSize);

                poolSize++;
            }

            if (IsGrid)
            {
                HighestCellLine = (HighestCellLine - 1 + Lines) % Lines;
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

            if (direction.x < 0 && CellPool[HighestCellIndex].MinX() < RecyclableViewBounds.max.x)
            {
                return RecycleLeftToRight();
            }
            else if (direction.x > 0 && CellPool[LowestCellIndex].MaxX() > RecyclableViewBounds.min.x)
            {
                return RecycleRightToleft();
            }
            return ZeroVector;
        }

        private Vector2 RecycleLeftToRight()
        {
            Recycling = true;

            int n = 0;
            float posX = IsGrid ? CellPool[HighestCellIndex].anchoredPosition.x : 0;
            float posY = 0;

            int additionalColoums = 0;

            while (CellPool[LowestCellIndex].MaxX() < RecyclableViewBounds.min.x && CurrentItemCount < DataSource.GetItemCount())
            {
                if (IsGrid)
                {
                    if (++HighestCellLine >= Lines)
                    {
                        n++;
                        HighestCellLine = 0;
                        posX = CellPool[HighestCellIndex].anchoredPosition.x + CellWidth;
                        additionalColoums++;
                    }

                    posY = -HighestCellLine * CellHeight;
                    CellPool[LowestCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++LowestCellLine >= Lines)
                    {
                        LowestCellLine = 0;
                        additionalColoums--;
                    }
                }
                else
                {
                    posX = CellPool[HighestCellIndex].anchoredPosition.x + CellPool[HighestCellIndex].sizeDelta.x;
                    CellPool[LowestCellIndex].anchoredPosition = new Vector2(posX, CellPool[LowestCellIndex].anchoredPosition.y);
                }

                DataSource.SetCell(CachedCells[LowestCellIndex], CurrentItemCount);

                HighestCellIndex = LowestCellIndex;
                LowestCellIndex = (LowestCellIndex + 1) % CellPool.Count;

                CurrentItemCount++;
                if (!IsGrid) n++;
            }

            if (IsGrid)
            {
                Content.sizeDelta += additionalColoums * Vector2.right * CellWidth;
                if (additionalColoums > 0)
                {
                    n -= additionalColoums;
                }
            }

            CellPool.ForEach(c => c.anchoredPosition -= n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x);
            Content.anchoredPosition += n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x;
            Recycling = false;
            return n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x;

        }

        private Vector2 RecycleRightToleft()
        {
            Recycling = true;

            int n = 0;
            float posX = IsGrid ? CellPool[LowestCellIndex].anchoredPosition.x : 0;
            float posY = 0;

            int additionalColoums = 0;
            while (CellPool[HighestCellIndex].MinX() > RecyclableViewBounds.max.x && CurrentItemCount > CellPool.Count)
            {
                if (IsGrid)
                {
                    if (--LowestCellLine < 0)
                    {
                        n++;
                        LowestCellLine = Lines - 1;
                        posX = CellPool[LowestCellIndex].anchoredPosition.x - CellWidth;
                        additionalColoums++;
                    }

                    posY = -LowestCellLine * CellHeight;
                    CellPool[HighestCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--HighestCellLine < 0)
                    {
                        HighestCellLine = Lines - 1;
                        additionalColoums--;
                    }
                }
                else
                {
                    posX = CellPool[LowestCellIndex].anchoredPosition.x - CellPool[LowestCellIndex].sizeDelta.x;
                    CellPool[HighestCellIndex].anchoredPosition = new Vector2(posX, CellPool[HighestCellIndex].anchoredPosition.y);
                    n++;
                }

                CurrentItemCount--;
                DataSource.SetCell(CachedCells[HighestCellIndex], CurrentItemCount - CellPool.Count);

                LowestCellIndex = HighestCellIndex;
                HighestCellIndex = (HighestCellIndex - 1 + CellPool.Count) % CellPool.Count;
            }

            if (IsGrid)
            {
                Content.sizeDelta += additionalColoums * Vector2.right * CellWidth;
                if (additionalColoums > 0)
                {
                    n -= additionalColoums;
                }
            }

            CellPool.ForEach(c => c.anchoredPosition += n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x);
            Content.anchoredPosition -= n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x;
            Recycling = false;
            return -n * Vector2.right * CellPool[LowestCellIndex].sizeDelta.x;
        }

        private void SetLeftAnchor(RectTransform rectTransform)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            Vector2 pos = IsGrid ? new Vector2(0, 1) : new Vector2(0, 0.5f);

            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;

            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(RecyclableViewBounds.min - new Vector3(0, 2000), RecyclableViewBounds.min + new Vector3(0, 2000));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(RecyclableViewBounds.max - new Vector3(0, 2000), RecyclableViewBounds.max + new Vector3(0, 2000));
        }
    }
}