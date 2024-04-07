using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RiseRakNet
{
    public class InfoStatus : MonoBehaviour
    {
        [SerializeField] private Text _info;

        public Text Info => _info;

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
