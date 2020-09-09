using System;
using JackSParrot.Utils;
using UnityEngine;

namespace JackSParrot.UI
{
    public enum UIAnimayionTypes
    {
        Move,
        Scale,
        Rotate,
        Fade
    }

    public enum UIAnimayionMethods
    {
        To,
        By
    }

    [System.Serializable]
    public class UITweenData
    {
        public GameObject Target = null;
        public UIAnimayionTypes AnimationType = UIAnimayionTypes.Move;
        public UIAnimayionMethods AnimationMethod = UIAnimayionMethods.To;
        public LeanTweenType EaseType = LeanTweenType.linear;
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
        LTDescr _tweenObj;
        UITweenData _playingData = null;
        public void Play(UITweenData data, Action onFinish = null)
        {
            if (data.Target == null)
            {
                SharedServices.GetService<ICustomLogger>()?.LogError("Tried to animate an object with no target");
                onFinish?.Invoke();
                return;
            }
            _playingData = data;

            switch (_playingData.AnimationType)
            {
                case UIAnimayionTypes.Move:
                    Move();
                    break;
                case UIAnimayionTypes.Scale:
                    Scale();
                    break;
                case UIAnimayionTypes.Rotate:
                    Rotate();
                    break;
                case UIAnimayionTypes.Fade:
                    Fade();
                    break;
            }

            _tweenObj.setDelay(_playingData.Delay);
            _tweenObj.setEase(_playingData.EaseType);

            if (_playingData.Loop)
            {
                _tweenObj.setLoopCount(-1);
            }
            if (_playingData.PingPong)
            {
                _tweenObj.setLoopPingPong();
            }
            if (onFinish != null)
            {
                _tweenObj.setOnComplete(onFinish);
            }
        }

        void Fade()
        {
            var cg = _playingData.Target.GetComponent<CanvasGroup>() ?? _playingData.Target.AddComponent<CanvasGroup>();
            if (_playingData.StartOffset)
            {
                cg.alpha = _playingData.From.x;
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to.x += cg.alpha;
            }
            _tweenObj = LeanTween.alphaCanvas(cg, to.x, _playingData.Duration);
        }

        void Move()
        {
            var rt = _playingData.Target.gameObject.GetComponent<RectTransform>();
            if (rt != null)
            {
                MoveRt(rt);
                return;
            }
            if (_playingData.StartOffset)
            {
                _playingData.Target.transform.localPosition = _playingData.From;
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to += _playingData.Target.transform.localPosition;
            }
            _tweenObj = LeanTween.move(_playingData.Target, to, _playingData.Duration);
        }

        void MoveRt(RectTransform rt)
        {
            if (_playingData.StartOffset)
            {
                rt.anchoredPosition = _playingData.From;
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to += new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0f);
            }
            _tweenObj = LeanTween.move(rt, to, _playingData.Duration);
        }

        void Scale()
        {
            var rt = _playingData.Target.GetComponent<Transform>();
            if (_playingData.StartOffset)
            {
                rt.localScale = _playingData.From;
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to += rt.localScale;
            }
            _tweenObj = LeanTween.scale(_playingData.Target, to, _playingData.Duration);
        }

        void Rotate()
        {
            var rt = _playingData.Target.GetComponent<RectTransform>();
            if (rt != null)
            {
                RotateRT(rt);
                return;
            }
            var t = _playingData.Target.GetComponent<Transform>();
            if (_playingData.StartOffset)
            {
                t.localRotation = Quaternion.Euler(_playingData.From);
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to.z += t.localRotation.z;
            }
            _tweenObj = LeanTween.rotate(_playingData.Target, to, _playingData.Duration);
        }

        void RotateRT(RectTransform rt)
        {
            if (_playingData.StartOffset)
            {
                rt.localRotation = Quaternion.Euler(_playingData.From);
            }
            var to = _playingData.To;
            if (_playingData.AnimationMethod == UIAnimayionMethods.By)
            {
                to.z += rt.localRotation.z;
            }
            _tweenObj = LeanTween.rotateAroundLocal(_playingData.Target, Vector3.forward, to.z, _playingData.Duration);
        }
    }
}