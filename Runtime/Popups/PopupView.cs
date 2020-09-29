using System;
using UnityEngine;
using JackSParrot.Utils;
using JackSParrot.Services.Audio;

namespace JackSParrot.UI
{
    public interface IPopupConfig
    {
        string PrefabName { get; }
    }

    public class PopupView : BaseView
    {
        [SerializeField] string _onAppearSound = null;
        [SerializeField] string _onHideSound = null;

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
                SharedServices.GetService<ICustomLogger>()?.LogError("Showing a popup not initialized");
            }
            if (!string.IsNullOrEmpty(_onAppearSound))
            {
                SharedServices.GetService<AudioService>()?.PlaySFX(_onAppearSound);
            }
        }

        public override void Hide(bool animated = true, Action onFinish = null)
        {
            base.Hide(animated, onFinish);
            if (!string.IsNullOrEmpty(_onHideSound))
            {
                SharedServices.GetService<AudioService>()?.PlaySFX(_onHideSound);
            }
        }

        protected override void OnHidden(Action callback)
        {
            base.OnHidden(callback);
            SharedServices.GetService<UIService>().PopPopup();
        }

        public void Close(Action callback = null)
        {
            Hide(true, callback);
        }
    }
}