using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace RiseRakNet
{
    public class GameController : MonoBehaviour
    {
        [CanBeNull] private static GameController _instance;
        [SerializeField] private Button _spawnButton;
        public static GameController Instance => _instance ?? throw new Exception("GameController not initialized");

        public Dictionary<ulong, PlayerController> PlayerControllers { get; } = new();

        public IEnumerable Init()
        {
            _instance = this;
            SpawnPlayer();

            yield break;
        }

        public void OnPlayerSpawn(ulong playerId, Vector3 pos)
        {
            var player = Resources.Load<GameObject>("Player/player");
            var controller = player.GetRequireComponent<PlayerController>();
            player.transform.position = pos;
            controller.IsLocal = playerId == RakClient.Guid;
            PlayerControllers.Add(playerId, controller);
        }
        public void OnPlayerMoved(ulong playerId, Vector3 move, Vector3 prevPos)
        {
            if (PlayerControllers.TryGetValue(playerId, out var player))
            {
                player.Move(move, prevPos);
            }
        }
        public void SpawnPlayer()
        {
            using var bsOut = PooledBitStream.GetBitStream();
            bsOut.Write((byte)RisePacketInfo.PLAYER_SPAWN_REQUEST);
            RakClient.Send(bsOut, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE);
        }
    }
}
