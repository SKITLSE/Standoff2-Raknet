using RiseRakNet.Misc;
using RiseRakNet.RakNet;
using UnityEngine;
using UnityEngine.UI;

namespace RiseRakNet.Menu
{
    public class MenuController : MonoBehaviour
    {
        private const ushort Port = 5056;

        [SerializeField] private Button _startButton;
        [SerializeField] private InfoStatus _status;
        
        private void Awake()
        {
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            if (RakClient.State == ClientState.IS_DISCONNECTED)
            {
                _status.Show();
                LazySingleton<Client>.Instance.Connect("127.0.0.1", Port, _status);
            }

        }

        private void OnDestroy()
        {
        }
    }
}
