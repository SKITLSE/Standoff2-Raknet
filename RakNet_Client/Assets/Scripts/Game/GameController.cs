using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RiseRakNet.Player;
using RiseRakNet.RakNet;
using UnityEngine;
using UnityEngine.UI;

namespace RiseRakNet.Game
{
    public class GameController : MonoBehaviour
    {
        [CanBeNull] private static GameController _instance;
        [SerializeField] private Button _spawnButton;
        public static GameController Instance => _instance ?? throw new Exception("GameController not initialized");

        public Dictionary<ulong, PlayerController> PlayerControllers { get; } = new();
        private void Awake()
        {
            _spawnButton.onClick.AddListener(SpawnPlayer);
        }
        public void Init()
        {
            _instance = this;
            SpawnPlayer();
        }

        public void OnPlayerSpawn(ulong playerId, Vector3 pos)
        {
            var player = Instantiate(Resources.Load<GameObject>("Player/player"));
            var controller = player.GetRequireComponent<PlayerController>();
            player.transform.position = pos;
            controller.IsLocal = playerId == RakClient.Guid;
            controller.SetMainCamera(Camera.main);
            PlayerControllers.Add(playerId, controller);
        }
        public void OnPlayerMoved(ulong playerId, StatePayload state)
        {
            if (PlayerControllers.TryGetValue(playerId, out var player))
            {
                player.OnServerMovementState(state);
            }
        }
        public void OnPlayerRemoved(ulong playerId)
        {
            if (!PlayerControllers.TryGetValue(playerId, out var player)) return;
            if (player.IsLocal) PlayerMainCamera.Instance.DetachFromPlayer();
            Destroy(player.gameObject);
            PlayerControllers.Remove(playerId);
        }
        public void SpawnPlayer()
        {
            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)RisePacketInfo.PLAYER_SPAWN_REQUEST);
            RakClient.Send(bsOut, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE);
        }

    }
}
