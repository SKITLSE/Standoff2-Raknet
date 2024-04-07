using System;
using RiseRakNet.Game;
using UnityEngine;

namespace RiseRakNet.Player
{
    public class PlayerMainCamera : MonoBehaviour
    {
        private Camera _camera;


        private static PlayerMainCamera _instance;

        public Transform Transform => transform;
        public Camera Camera => _camera;
        public PlayerController PlayerController { get; private set; }

        public static PlayerMainCamera Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var playerMainCamera = FindObjectOfType<PlayerMainCamera>() ?? throw new Exception("PlayerMainCamera not found");
                return _instance = playerMainCamera;
            }
        }

        private void Awake()
        {
            _instance = this;
            _camera = gameObject.GetRequireComponent<Camera>();
        }
        
        private void OnDestroy()
        {
            _instance = null;
        }

        public void AttachToPlayer(PlayerController playerController, Transform cameraPlaceholder)
        {
            if (PlayerController != null) DetachFromPlayer();
            PlayerController = playerController;
            transform.SetParent(cameraPlaceholder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void DetachFromPlayer()
        {
            transform.SetParent(null);
            PlayerController?.ResetMainCamera();
            PlayerController = null;
        }
    }
}