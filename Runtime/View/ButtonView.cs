﻿using JackSParrot.Services.Audio;
using System;
using JackSParrot.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JackSParrot.UI
{
    public class ButtonView : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField]
        DOTween.UITweenData _pointerDownTweenData = null;
        [SerializeField]
        DOTween.UITweenData _pointerUpTweenData = null;
        [SerializeField]
        bool _enabled = true;
        [SerializeField]
        TMPro.TMP_Text _textTMP = null;
        [SerializeField]
        Image _image = null;
        [SerializeField]
        ClipId _onClickSound;
        [SerializeField]
        Sprite _disabledImage = null;
        [SerializeField]
        UnityEvent _onClick = null;
        Sprite            _activeImage = null;
        DOTween.UITweener _tweener     = new DOTween.UITweener();

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (_activeImage == null)
                {
                    _activeImage = _image.sprite;
                }

                _image.sprite = _enabled ? _activeImage : _disabledImage;
            }
        }

        public string Text
        {
            get { return _textTMP.text; }

            set
            {
                if (_textTMP != null)
                {
                    _textTMP.text = value;
                }
            }
        }

        public Color TextColor
        {
            get { return _textTMP.color; }

            set
            {
                if (_textTMP != null)
                {
                    _textTMP.color = value;
                }
            }
        }

        public Sprite ImageSprite
        {
            set { _image.sprite = value; }
        }

        public Image Image
        {
            get { return _image; }
        }

        public Color Color
        {
            get { return _image.color; }
            set { _image.color = value; }
        }

        public event Action OnClick;

        void Awake()
        {
            if (_image != null && _activeImage == null)
            {
                _activeImage = _image.sprite;
            }

            if (_pointerDownTweenData.Target == null)
            {
                _pointerDownTweenData.Target = gameObject;
            }

            if (_pointerUpTweenData.Target == null)
            {
                _pointerUpTweenData.Target = gameObject;
            }
        }

        void Onclicked()
        {
            if (!_enabled) return;
            OnClick?.Invoke();
            _onClick?.Invoke();
            if (_onClickSound.IsValid())
            {
                ServiceLocator.GetService<AudioService>()?.PlaySfx(_onClickSound);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_enabled) return;
            _tweener.Play(_pointerUpTweenData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_enabled) return;
            _tweener.Play(_pointerDownTweenData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Onclicked();
        }
    }
}