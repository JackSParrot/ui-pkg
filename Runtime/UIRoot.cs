using UnityEngine;

namespace JackSParrot.UI
{
    public class UIRoot : MonoBehaviour 
    {
        [SerializeField] RectTransform _hudTransform = null;
        [SerializeField] RectTransform _popupsTransform = null;
        [SerializeField] RectTransform _notificationsTransform = null;

        public void AddNotification(Notification notification)
        {
            if(_notificationsTransform.childCount > 0)
            {
                var rt = notification.GetComponent<RectTransform>();
                for(int i = 0; i < _notificationsTransform.childCount; ++i)
                {
                    var current = _notificationsTransform.GetChild(i);
                    float height = Screen.safeArea.height * (rt.anchorMax.y - rt.anchorMin.y);
                    current.localPosition += new Vector3(0.0f, height, 0.0f);
                }
            }
            notification.transform.SetParent(_notificationsTransform, false);
        }

        public void AddPopup(PopupView popup)
        {
            popup.transform.SetParent(_popupsTransform, false);
        }

        public void SetHUD(BaseView hud)
        {
            hud.transform.SetParent(_hudTransform, false);
        }
    }
}