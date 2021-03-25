using System;
using DG.Tweening;
using JackSParrot.Utils;
using UnityEngine;

namespace JackSParrot.UI.DOTween
{
    public enum UIAnimationTypes
    {
        Move,
        Scale,
        Rotate,
        Fade
    }

    public enum UIAnimationMethods
    {
        To,
        By
    }

    [System.Serializable]
    public class UITweenData
    {
        public GameObject Target = null;
        public UIAnimationTypes AnimationType = UIAnimationTypes.Move;
        public UIAnimationMethods AnimationMethod = UIAnimationMethods.To;
        public Ease EaseType = Ease.Linear;
        public float Duration = 0f;
        public float Delay = 0f;
        public bool Loop = false;
        public bool PingPong = false;
        public bool StartOffset = true;
        public Vector3 From = Vector3.zero;
        public Vector3 To = Vector3.one;
    }

    public class UITweener
    {
        Tween _tweenObj;
        UITweenData _playingData = null;

        public void Play(UITweenData data, Action onFinish = null)
        {
            Stop();
            if (data.Target == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Tried to animate an object with no target");
                onFinish?.Invoke();
                return;
            }

            _playingData = data;

            switch (_playingData.AnimationType)
            {
                case UIAnimationTypes.Move:
                    Move();
                    break;
                case UIAnimationTypes.Scale:
                    Scale();
                    break;
                case UIAnimationTypes.Rotate:
                    Rotate();
                    break;
                case UIAnimationTypes.Fade:
                    Fade();
                    break;
            }

            _tweenObj.SetDelay(_playingData.Delay);
            _tweenObj.SetEase(_playingData.EaseType);

            if (_playingData.Loop)
            {
                _tweenObj.SetLoops(-1);
            }

            if (_playingData.PingPong)
            {
                _tweenObj.SetLoops(-1, LoopType.Yoyo);
            }

            if (onFinish != null)
            {
                _tweenObj.OnComplete(() => onFinish?.Invoke());
            }
        }

        public void Stop()
        {
            if (_tweenObj != null)
            {
                _tweenObj.Kill();
                _tweenObj = null;
            }
        }

        void Fade()
        {
            if (_playingData.Target.TryGetComponent(out RectTransform rt))
            {
                FadeRT();
                return;
            }

            if (!_playingData.Target.TryGetComponent(out Renderer renderer))
            {
                return;
            }

            Material material = renderer.material;
            if (_playingData.StartOffset)
            {
                var color = material.color;
                 color.a = _playingData.From.x;
                 material.color = color;
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to.x += material.color.a;
            }

            _tweenObj = material.DOFade(to.x, _playingData.Duration);
        }

        void FadeRT()
        {
            if (!_playingData.Target.TryGetComponent(out CanvasGroup cg))
            {
                cg = _playingData.Target.AddComponent<CanvasGroup>();
            }

            if (_playingData.StartOffset)
            {
                cg.alpha = _playingData.From.x;
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to.x += cg.alpha;
            }

            _tweenObj = cg.DOFade(to.x, _playingData.Duration);
        }

        void Move()
        {
            if (_playingData.Target.gameObject.TryGetComponent(out RectTransform rt))
            {
                MoveRt(rt);
                return;
            }

            if (_playingData.StartOffset)
            {
                _playingData.Target.transform.localPosition = _playingData.From;
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to += _playingData.Target.transform.localPosition;
            }

            _tweenObj = _playingData.Target.transform.DOLocalMove(to, _playingData.Duration);
        }

        void MoveRt(RectTransform rt)
        {
            if (_playingData.StartOffset)
            {
                rt.localPosition = _playingData.From;
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to += rt.localPosition;
            }

            _tweenObj = rt.DOLocalMove(to, _playingData.Duration);
        }

        void Scale()
        {
            Transform transform = _playingData.Target.transform;
            if (_playingData.StartOffset)
            {
                transform.localScale = _playingData.From;
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to += transform.localScale;
            }

            _tweenObj = transform.DOScale(to, _playingData.Duration);
        }

        void Rotate()
        {
            if (_playingData.Target.gameObject.TryGetComponent(out RectTransform rt))
            {
                RotateRT(rt);
                return;
            }

            Transform transform = _playingData.Target.transform;
            if (_playingData.StartOffset)
            {
                transform.localRotation = Quaternion.Euler(_playingData.From);
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to.z += transform.localRotation.z;
            }

            _tweenObj = transform.DOLocalRotate(to, _playingData.Duration);
        }

        void RotateRT(RectTransform rt)
        {
            if (_playingData.StartOffset)
            {
                rt.localRotation = Quaternion.Euler(_playingData.From);
            }

            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimationMethods.By)
            {
                to += rt.localRotation.eulerAngles;
            }

            _tweenObj = _playingData.Target.transform.DOLocalRotate(to, _playingData.Duration);
        }
    }
}