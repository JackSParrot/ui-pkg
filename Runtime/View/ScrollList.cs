using System;
using UnityEngine;
using UnityEngine.UI;

namespace JackSParrot.UI
{
    public interface IScrollListDataSource
    {
        int GetItemCount();
        void SetCell(Transform cell, int index);
    }

    public class ScrollList : ScrollRect
    {
        public enum EDirectionType
        {
            Vertical,
            Horizontal
        }

        public bool IsGrid;
        public EDirectionType Direction;
        public RectTransform PrototypeCell;

        [SerializeField] int _segments = 0;

        RecyclingSystem _recyclableScrollRect;

        public int Segments
        {
            set
            {
                _segments = Math.Max(value, 2);
            }
            get
            {
                return _segments;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            vertical = true;
            horizontal = false;
        }

        protected override void Start()
        {
            base.Start();
            vertical = Direction == EDirectionType.Vertical;
            horizontal = Direction == EDirectionType.Horizontal;
        }

        public void Initialize(IScrollListDataSource dataSource)
        {
            vertical = Direction == EDirectionType.Vertical;
            horizontal = Direction == EDirectionType.Horizontal;

            _recyclableScrollRect = vertical ? new VerticalRecyclingSystem() : new HorizontalRecyclingSystem() as RecyclingSystem;

            onValueChanged.RemoveListener(OnValueChangedListener);
            _recyclableScrollRect.Initialize(PrototypeCell, viewport, content, dataSource, IsGrid, Segments);
            onValueChanged.AddListener(OnValueChangedListener);
        }

        public void RepaintItems()
        {
            _recyclableScrollRect.RepaintItems();
        }

        public void OnValueChangedListener(Vector2 direction)
        {
            m_ContentStartPosition += _recyclableScrollRect.OnValueChangedListener(velocity);
        }

        void OnDrawGizmos()
        {
            (_recyclableScrollRect as VerticalRecyclingSystem)?.OnDrawGizmos();
            (_recyclableScrollRect as HorizontalRecyclingSystem)?.OnDrawGizmos();
        }
    }
}