using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RiseRakNet
{
    public class MenuController : MonoBehaviour
    {
        private const ushort Port = 5056;

        [SerializeField] private int _tickRate = 64;
        
        private void Awake()
        {
            Application.targetFrameRate = 64;
            var inst = LazySingleton<Server>.Instance;
            inst.StartServer(Port, 10, "TestLevel", _tickRate);
        }


        private void OnDestroy()
        {
            LazySingleton<Server>.Instance.StopServer();
        }
    }
}
