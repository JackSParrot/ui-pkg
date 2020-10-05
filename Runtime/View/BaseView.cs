using UnityEngine;
using System;

namespace JackSParrot.UI
{
    public class BaseView : MonoBehaviour
    {
        [SerializeField] ViewAnimationInfo _animationInfo;

        [HideInInspector]
        public bool IsShown = false;

        protected virtual void UnityAwake()
        {
            IsShown = false;
            gameObject.SetActive(false);
            if(_animationInfo == null)
            {
                _animationInfo = GetComponent<ViewAnimationInfo>();
            }
        }

        void Awake()
        {
            UnityAwake();
        }

        protected virtual void UnityOnEnable()
        {

        }

        void OnEnable()
        {
            UnityOnEnable();
        }

        protected virtual void UnityOnDisable()
        {

        }

        void OnDisable()
        {
            UnityOnDisable();
        }

        public virtual void Show(bool animated = true, Action onFinish = null)
        {
            if (IsShown) return;
            IsShown = true;

            gameObject.SetActive(true);
            if (animated && _animationInfo != null)
            {
                _animationInfo.Tweener.Play(_animationInfo.ShowTweener, onFinish);
            }
            else
            {
                onFinish?.Invoke();
            }
        }

        public virtual void Hide(bool animated = true, Action onFinish = null)
        {
            if (!IsShown) return;
            IsShown = false;

            if (animated && _animationInfo != null)
            {
                _animationInfo.Tweener.Play(_animationInfo.HideTweener, () => OnHidden(onFinish));
            }
            else
            {
                OnHidden(onFinish);
            }
        }

        protected virtual void OnHidden(Action callback)
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }
    }
}
