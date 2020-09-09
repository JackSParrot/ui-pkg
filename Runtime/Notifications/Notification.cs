using System;
using UnityEngine;
using System.Collections;

namespace JackSParrot.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Notification : BaseView
    {
        [SerializeField]
        TMPro.TMP_Text _text = null;
        CanvasGroup _group = null;

        public string Text
        {
            get
            {
                return _text.text;
            }
            set
            {
                _text.text = value;
            }
        }

        IEnumerator LifeCycleCoroutine()
        {
            _group.alpha = 1f;
            yield return new WaitForSeconds(2.0f);
            Vector3 target = transform.localPosition + new Vector3(0f, Screen.safeArea.height * 0.5f, 0f);
            Vector3 origin = transform.localPosition;
            float duration = 1f;
            float remaining = duration;
            while(remaining > 0f)
            {
                float factor = Mathf.Max(0f, remaining / duration);
                var factorInv = 1f - factor;
                transform.localPosition = Vector3.Lerp(origin, target, factorInv);
                _group.alpha = factor;
                remaining -= Time.deltaTime;
                yield return null;
            }
            Hide(true);
        }

        protected override void OnHidden(Action callback)
        {
            base.OnHidden(callback);
            Destroy(gameObject);
        }

        protected override void UnityAwake()
        {
            base.UnityAwake();
            _group = GetComponent<CanvasGroup>();
        }

        public override void Show(bool animated = true, Action onFinish = null)
        {
            base.Show(animated, onFinish);
            StartCoroutine(LifeCycleCoroutine());
        }
    }
}
