using System;
using UnityEngine;
using JackSParrot.Utils;

namespace JackSParrot.UI
{
    public class GenericPopupView : PopupView
    {
        public class GenericPopupConfig : IPopupConfig
        {
            public string PrefabAddress => "MessagePopup";
            public string Title;
            public string Message;
            public string OkButtonText;
            public bool CloseButton;
            public Action OnAccept;
            public Action OnCancel;
        }

        [SerializeField] TMPro.TMP_Text _titleText = null;
        [SerializeField] TMPro.TMP_Text _messageText = null;
        [SerializeField] ButtonView _okButton = null;
        [SerializeField] ButtonView _closeButton = null;

        GenericPopupConfig _popupConfig = null;

        public override void Initialize(IPopupConfig config)
        {
            base.Initialize(config);
            _popupConfig = config as GenericPopupConfig;
            if(_popupConfig == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Popup config is wrong");
            }
            _titleText.text = _popupConfig.Title;
            _messageText.text = _popupConfig.Message;
            _okButton.Text = _popupConfig.OkButtonText;
            _closeButton.gameObject.SetActive(_popupConfig.CloseButton);
        }

        void OkClicked()
        {
            Close(_popupConfig.OnAccept);
        }

        void CancelClicked()
        {
            Close(_popupConfig.OnCancel);
        }

        protected override void UnityOnEnable()
        {
            base.UnityOnEnable();
            _okButton.OnClick += OkClicked;
            _closeButton.OnClick += CancelClicked;
        }

        protected override void UnityOnDisable()
        {
            base.UnityOnDisable();
            _okButton.OnClick -= OkClicked;
            _closeButton.OnClick -= CancelClicked;
        }
    }
}