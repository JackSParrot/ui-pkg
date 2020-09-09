using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using JackSParrot.Utils;

namespace JackSParrot.UI
{
    public class UIService : System.IDisposable
    {
        UIRoot _uiRoot;
        UIRoot _uiRootInstance
        {
            get
            {
                if(_uiRoot == null)
                {
                    _uiRoot = UnityEngine.Object.FindObjectOfType<UIRoot>();
                    if(_uiRoot == null)
                    {
                        CreateUIRoot();
                    }
                }
                return _uiRoot;
            }
        }
        
        readonly Stack<PopupView> _currentPopups = new Stack<PopupView>();
        BaseView _currentHud = null;

        public void ShowMessage(string message)
        {
            var notification = Object.Instantiate(Resources.Load<GameObject>("Notification")).GetComponent<Notification>();
            notification.Text = message;
            notification.Show();
            _uiRootInstance.AddNotification(notification);
        }

        public void PushPopup<T>(PopupConfig config, System.Action<T> onLoaded) where T : PopupView
        {
            Addressables.InstantiateAsync(config.PrefabName).Completed += r => OnPopupLoaded(r, config, onLoaded); 
        }

        private void OnPopupLoaded<T>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj, PopupConfig config, System.Action<T> onLoaded) where T : PopupView
        {
            var res = obj.Result;
            if(res == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Could not load popup " + config.PrefabName);
                onLoaded?.Invoke(null);
                return;
            }
            var popup = res.GetComponent<T>();
            popup.Initialize(config);
            _currentPopups.Push(popup);
            _uiRootInstance.AddPopup(popup);
            onLoaded?.Invoke(popup);
        }

        public void PopPopup()
        {
            UnityEngine.Object.Destroy(_currentPopups.Pop().gameObject);
        }

        public PopupView CurrentPopup()
        {
            return _currentPopups.Count > 0 ? _currentPopups.Peek() : null;
        }


        public void SetHUD<T>(string prefabName, System.Action<T> onLoaded) where T : BaseView
        {
            Addressables.InstantiateAsync(prefabName).Completed += r => OnHUDLoaded(r, onLoaded);
        }

        private void OnHUDLoaded<T>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj, System.Action<T> onLoaded) where T : BaseView
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
            _uiRootInstance.SetHUD(hud);
            _currentHud = hud;
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
            if(_uiRoot != null)
            {
                UnityEngine.Object.Destroy(_uiRoot.gameObject);
            }
        }
    }
}