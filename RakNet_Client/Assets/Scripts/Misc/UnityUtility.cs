using UnityEngine;

namespace RiseRakNet.Misc
{
    public class UnityUtility
    {
        public static Transform FindDeepChild(Transform aParent, string aName)
        {
            var transform = aParent.Find(aName);
            if (transform != null)
            {
                return transform;
            }
            foreach (Transform item in aParent)
            {
                transform = FindDeepChild(item, aName);
                if (transform != null)
                {
                    return transform;
                }
            }
            return null;
        }

        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            var componentsInChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (var transform in componentsInChildren)
            {
                transform.gameObject.layer = layer;
            }
        }
    }
}