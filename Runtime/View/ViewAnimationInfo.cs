using UnityEngine;

namespace JackSParrot.UI
{
    public class ViewAnimationInfo : MonoBehaviour
    {
        public DOTween.UITweenData ShowTweener = null;
        public DOTween.UITweenData HideTweener = null;

        public DOTween.UITweener Tweener = new DOTween.UITweener();
    }
}