using UnityEngine;

namespace JackSParrot.UI
{
    public class ViewAnimationInfo : MonoBehaviour
    {
        public UITweenData ShowTweener = null;
        public UITweenData HideTweener = null;

        public UITweener Tweener = new UITweener();
    }
}