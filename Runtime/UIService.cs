using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using JackSParrot.Utils;

namespace JackSParrot.UI
{
    public class PopupOpenedEvent
    {
        public PopupOpenedEvent(string address)
        {
            PopupAddress = address;
        }

        public string PopupAddress;
    }

    public class UIService : System.IDisposable
    {
        UIRoot _uiRoot;
        UIRoot _uiRootInstance
        {
            get
            {
                if (_uiRoot == null)
                {
                    _uiRoot = UnityEngine.Object.FindObjectOfType<UIRoot>();
                    if (_uiRoot == null)
                    {
                        CreateUIRoot();
                    }
                }

                return _uiRoot;
            }
        }

        readonly Stack<PopupView> _currentPopups = new Stack<PopupView>();
        BaseView                  _currentHud    = null;

        public void ShowMessage(string message)
        {
            var notification = Object.Instantiate(Resources.Load<GameObject>("Notification"))
                .GetComponent<Notification>();
            notification.Text = message;
            notification.Show();
            _uiRootInstance.AddNotification(notification);
        }

        public void PushPopup(IPopupConfig config)
        {
            Addressables.InstantiateAsync(config.PrefabAddress).Completed += r => OnPopupLoaded(r, config);
        }

        public void PushPopup<T>(IPopupConfig config, System.Action<T> onPopupLoaded) where T : PopupView
        {
            Addressables.InstantiateAsync(config.PrefabAddress).Completed +=
                r => onPopupLoaded(OnPopupLoaded(r, config) as T);
        }

        private PopupView OnPopupLoaded(
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj, IPopupConfig config)
        {
            var res = obj.Result;
            if (res == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Could not load popup " + config.PrefabAddress);
                return null;
            }

            var popup = res.GetComponent<PopupView>();
            popup.Initialize(config);
            _currentPopups.Push(popup);
            _uiRootInstance.AddPopup(popup);
            popup.Show();
            SharedServices.GetService<EventDispatcher>()?.Raise(new PopupOpenedEvent(config.PrefabAddress));
            return popup;
        }

        public void PopPopup()
        {
            UnityEngine.Object.Destroy(_currentPopups.Pop().gameObject);
        }

        public PopupView CurrentPopup()
        {
            return _currentPopups.Count > 0 ? _currentPopups.Peek() : null;
        }

        public void SetHUD<T>(string prefabName, System.Action<T> onLoaded = null) where T : BaseView
        {
            Addressables.InstantiateAsync(prefabName).Completed += r => OnHUDLoaded(r, onLoaded);
        }

        private void OnHUDLoaded<T>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj,
            System.Action<T> onLoaded) where T : BaseView
        {
            var res = obj.Result;
            if (res == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Could not load HUD");
                onLoaded?.Invoke(null);
                return;
            }

            var hud = res.GetComponent<T>();
            SetHUD(hud);
            onLoaded?.Invoke(hud);
        }

        void SetHUD(BaseView hud)
        {
            if (hud == null) return;
            if (_currentHud != null)
            {
                UnityEngine.Object.Destroy(_currentHud.gameObject);
            }

            _currentHud = hud;
            _uiRootInstance.SetHUD(hud);
            hud.Show();
        }

        public void HideHUD(System.Action onDone = null)
        {
            if (_currentHud == null)
            {
                onDone?.Invoke();
                return;
            }

            _currentHud.Hide(true, () =>
            {
                UnityEngine.Object.Destroy(_currentHud.gameObject);
                _currentHud = null;
                onDone?.Invoke();
            });
        }

        public BaseView CurrentHud()
        {
            return _currentHud;
        }

        void CreateUIRoot()
        {
            var go = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UIRoot"));
            _uiRoot = go.GetComponent<UIRoot>();
            UnityEngine.Object.DontDestroyOnLoad(_uiRoot.gameObject);
        }

        public void Dispose()
        {
            if (_uiRoot != null)
            {
                UnityEngine.Object.Destroy(_uiRoot.gameObject);
            }
        }
    }
}