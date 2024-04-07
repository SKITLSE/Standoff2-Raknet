using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RiseRakNet
{
    public class Gizmos : MonoBehaviour
    {
        public Color Color = Color.blue;
        public Vector3 Scale;
        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = Color;
            UnityEngine.Gizmos.DrawWireCube(transform.position, Scale);
        }
    }
}
