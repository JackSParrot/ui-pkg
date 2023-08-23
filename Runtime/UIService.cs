using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using JackSParrot.UI;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace JackSParrot.Services
{
	public class PopupOpenedEvent
	{
		public PopupOpenedEvent(string address)
		{
			PopupAddress = address;
		}

		public string PopupAddress;
	}

	public class UIService: AService
	{
		UIRoot _uiRoot;

		readonly Stack<PopupView> _currentPopups = new Stack<PopupView>();
		BaseView                  _currentHud    = null;

		public void ShowMessage(string message)
		{
			Notification notification = Object.Instantiate(Resources.Load<GameObject>("Notification"))
											  .GetComponent<Notification>();
			notification.Text = message;
			notification.Show();
			_uiRoot.AddNotification(notification);
		}

		public void PushPopup(IPopupConfig config)
		{
			Addressables.InstantiateAsync(config.PrefabAddress).Completed += r => OnPopupLoaded(r, config);
		}

		public void PushPopup<T>(IPopupConfig config, System.Action<T> onPopupLoaded) where T: PopupView
		{
			Addressables.InstantiateAsync(config.PrefabAddress).Completed +=
				r => onPopupLoaded(OnPopupLoaded(r, config) as T);
		}

		private PopupView OnPopupLoaded(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj, IPopupConfig config)
		{
			GameObject res = obj.Result;
			if (res == null)
			{
				Debug.LogError("Could not load popup " + config.PrefabAddress);
				return null;
			}

			PopupView popup = res.GetComponent<PopupView>();
			popup.Initialize(config);
			_currentPopups.Push(popup);
			_uiRoot.AddPopup(popup);
			popup.Show();
			ServiceLocator.GetService<GlobalEventBus>()?.Raise(new PopupOpenedEvent(config.PrefabAddress));
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

		public void SetHUD<T>(string prefabName, System.Action<T> onLoaded = null) where T: BaseView
		{
			Addressables.InstantiateAsync(prefabName).Completed += r => OnHUDLoaded(r, onLoaded);
		}

		private void OnHUDLoaded<T>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj,
									System.Action<T> onLoaded) where T: BaseView
		{
			GameObject res = obj.Result;
			if (res == null)
			{
				Debug.LogError("Could not load HUD");
				onLoaded?.Invoke(null);
				return;
			}

			T hud = res.GetComponent<T>();
			SetHUD(hud);
			onLoaded?.Invoke(hud);
		}

		void SetHUD(BaseView hud)
		{
			if (hud == null)
				return;
			if (_currentHud != null)
			{
				UnityEngine.Object.Destroy(_currentHud.gameObject);
			}

			_currentHud = hud;
			_uiRoot.SetHUD(hud);
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

		UIRoot CreateUIRoot()
		{
			GameObject go = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UIRoot"));
			UnityEngine.Object.DontDestroyOnLoad(go);
			return go.GetComponent<UIRoot>();
		}

		public override void Cleanup()
		{
			if (_uiRoot != null)
			{
				UnityEngine.Object.Destroy(_uiRoot.gameObject);
			}
		}

		public override List<Type> GetDependencies()
		{
			return null;
		}

		public override IEnumerator Initialize()
		{
			_uiRoot = UnityEngine.Object.FindObjectOfType<UIRoot>() ?? CreateUIRoot();
			Status = EServiceStatus.Initialized;
			yield return null;
		}
	}
}
