using System;
using JackSParrot.Services;
using UnityEngine;
using JackSParrot.Utils;
using JackSParrot.Services.Audio;

namespace JackSParrot.UI
{
    public interface IPopupConfig
    {
        string PrefabAddress { get; }
    }

    public class PopupView : BaseView
    {
        [SerializeField]
        ClipId _onAppearSound;
        [SerializeField]
        ClipId _onHideSound;

        IPopupConfig _config = null;

        public virtual void Initialize(IPopupConfig config)
        {
            _config = config;
        }

        public override void Show(bool animated = true, Action onFinish = null)
        {
            base.Show(animated, onFinish);
            if (_config == null)
            {
                Debug.LogError("Showing a popup not initialized");
            }

            if (_onAppearSound.IsValid())
            {
                ServiceLocator.GetService<AudioService>()?.PlaySfx(_onAppearSound);
            }
        }

        public override void Hide(bool animated = true, Action onFinish = null)
        {
            base.Hide(animated, onFinish);
            if (_onHideSound.IsValid())
            {
                ServiceLocator.GetService<AudioService>()?.PlaySfx(_onHideSound);
            }
        }

        protected override void OnHidden(Action callback)
        {
            ServiceLocator.GetService<UIService>().PopPopup();
            base.OnHidden(callback);
        }

        public void Close(Action callback = null)
        {
            Hide(true, callback);
        }
    }
}