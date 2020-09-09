using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using JackSParrot.Utils;

namespace JackSParrot.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollListView : MonoBehaviour
    {
        [SerializeField] GameObject _replicationPrefab = null;
        [SerializeField] GameObject _container = null;
        readonly List<GameObject> _elements = new List<GameObject>();

        public int Count => _elements.Count;

        void Awake()
        {
            _replicationPrefab.SetActive(false);
            _replicationPrefab.transform.SetParent(transform.parent);
        }

        public GameObject AddElement()
        {
            var clone = Instantiate(_replicationPrefab);
            clone.SetActive(true);
            clone.transform.SetParent(_container.transform, false);
            _elements.Add(clone);
            return _elements[Count - 1];
        }

        public GameObject AddElementAt(int idx)
        {
            var clone = Instantiate(_replicationPrefab);
            clone.SetActive(true);
            clone.transform.SetParent(_container.transform, false);
            clone.transform.SetSiblingIndex(idx);
            _elements.Insert(idx, clone);
            return clone;
        }

        public void RemoveElement(GameObject obj)
        {
            if(obj != null && _elements.Contains(obj))
            {
                _elements.Remove(obj);
                Destroy(obj);
            }
        }

        public GameObject GetElementAt(int idx)
        {
            if(idx >= 0 && idx < Count)
            {
                return _elements[idx];
            }
            SharedServices.GetService<ICustomLogger>()?.LogError("Index out of bounds");
            return null;
        }

        public void Clear()
        {
            for(int i = 0; i < _elements.Count; ++i)
            {
                Destroy(_elements[i].gameObject);
            }
            _elements.Clear();
        }

        public void RemoveElementAt(int idx)
        {
            RemoveElement(GetElementAt(idx));
        }

        public T AddElement<T>() where T : MonoBehaviour
        {
            return AddElement().GetComponent<T>();
        }

        public T AddElementAt<T>(int idx) where T : MonoBehaviour
        {
            return AddElementAt(idx).GetComponent<T>();
        }

        public T GetElementAt<T>(int idx) where T : MonoBehaviour
        {
            var retVal = GetElementAt(idx);
            return retVal == null ? null : retVal.GetComponent<T>();
        }
    }
}