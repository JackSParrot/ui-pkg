﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace JackSParrot.UI
{
    public class UITweenerComponent : MonoBehaviour
    {
        [SerializeField] UITweenData _data = null;
        [SerializeField] bool _showOnEnable = true;
        [SerializeField] UnityEvent _onComplete = new UnityEvent();
        UITweener _tweener = new UITweener();

        void OnEnable()
        {
            if (_showOnEnable)
            {
                _tweener.Play(_data, () => _onComplete?.Invoke());
            }
        }

        public void Play(Action callback)
        {
            _tweener.Play(_data, () =>
            {
                _onComplete?.Invoke();
                callback?.Invoke();
            });
        }
    }
}
