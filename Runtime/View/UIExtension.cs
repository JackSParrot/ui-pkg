using UnityEngine;

namespace JackSParrot.UI
{
    public static class UIExtension
    {
        static readonly Vector3[] corners = new Vector3[4];
        public static Vector3[] GetCorners(this RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(corners);
            return corners;
        }
        public static float MaxY(this RectTransform rectTransform)
        {
            GetCorners(rectTransform);
            return corners[1].y;
        }
        public static float MinY(this RectTransform rectTransform)
        {
            GetCorners(rectTransform);
            return corners[0].y;
        }
        public static float MaxX(this RectTransform rectTransform)
        {
            GetCorners(rectTransform);
            return corners[2].x;
        }
        public static float MinX(this RectTransform rectTransform)
        {
            GetCorners(rectTransform);
            return corners[0].x;
        }
    }
}