using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.UI
{
    public abstract class RecyclingSystem
    {
        protected RectTransform Viewport;
        protected RectTransform Content;
        protected RectTransform PrototypeCell;
        protected bool IsGrid;
        protected float MinPoolCoverage = 1.5f;
        protected int MinPoolSize = 10;
        protected float RecyclingThreshold = .2f;
        protected Vector2 ZeroVector = Vector2.zero;

        protected List<RectTransform> CellPool;
        protected List<Transform> CachedCells;
        protected Bounds RecyclableViewBounds;
        protected Vector3[] Corners = new Vector3[4];
        protected bool Recycling;
        protected float CellWidth;
        protected float CellHeight;
        protected int CurrentItemCount;
        protected int Lines;
        protected int LowestCellIndex;
        protected int HighestCellIndex;
        protected int LowestCellLine;
        protected int HighestCellLine;

        protected IScrollListDataSource DataSource;
        public abstract Vector2 OnValueChangedListener(Vector2 direction);

        public void RepaintItems()
        {
            int itemCount = DataSource.GetItemCount();
            if (CachedCells.Count > itemCount)
            {
                Initialize(PrototypeCell, Viewport, Content, DataSource, IsGrid, Lines);
            }
            else
            {
                if (CurrentItemCount >= itemCount)
                {
                    CurrentItemCount = itemCount - 1;
                }
                for (int i = 0; i < CachedCells.Count; ++i)
                {
                    int idx = i + Mathf.Max(0, CurrentItemCount - CellPool.Count);
                    if (idx < itemCount)
                    {
                        DataSource.SetCell(CachedCells[i], idx);
                    }
                    else
                    {
                        CachedCells[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public virtual void Initialize(RectTransform prototypeCell, RectTransform viewport, RectTransform content, IScrollListDataSource dataSource, bool isGrid, int lines)
        {
            PrototypeCell = prototypeCell;
            Viewport = viewport;
            Content = content;
            DataSource = dataSource;
            IsGrid = isGrid;
            Lines = isGrid ? lines : 1;
            RecyclableViewBounds = new Bounds();
        }
    }
}