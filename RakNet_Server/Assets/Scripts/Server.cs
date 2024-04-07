using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace RiseRakNet
{
    public class PlayerData
    {
        public ulong Guid;
        public string PlayerName;
        public PlayerServerController PlayerServerController;

        public PlayerData(ulong guid, string username)
        {
            Guid = guid;
            PlayerName = username;
        }
    }
    public class Server : LazySingleton<Server>, IRakServer
    {
        private readonly List<PlayerData> _clients = new();

        public readonly NetworkInfo Info = new();

        private GameObject _player;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            RakServer.RegisterInterface(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RakServer.UnRegisterInterface(this);
        }

        public void StartServer(ushort port, ushort maxConnect, string map, int tickRate)
        {
            SceneManager.LoadScene(map, LoadSceneMode.Single);
            _player = Resources.Load<GameObject>("Player/serverPlayer");
            Info.TickRate = tickRate;
            RakServer.Start(port: port, max_connections: maxConnect);
        }

        public void StopServer()
        {
        }
        private void RemoteClientData(ulong guid)
        {
            for (var i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Guid != guid) continue;
                Destroy(_clients[i].PlayerServerController.gameObject);
                _clients.RemoveAt(i);
                break;
            }

            Info.Players.RemoveAll(a => a.Guid == guid);
        }

        void IRakServer.OnConnected(ushort connectionIndex, ulong guid)
        {
            Debug.Log("[SampleServer] Client connected with guid " + guid + " [IP: " + RakServer.GetAddress(guid, true) + "]");

            using var bitStream = PooledBitStream.GetBitStream();
            bitStream.Write((byte)SamplePacketID.CLIENT_DATA_REQUEST);
            RakServer.SendToClient(bitStream, guid, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
        }

        void IRakServer.OnDisconnected(ushort connectionIndex, ulong guid, DisconnectReason reason, string message)
        {
            if (_clients[connectionIndex] != null && _clients[connectionIndex].Guid == guid)
            {
                Debug.Log("[Server] Client " + _clients[connectionIndex].PlayerName + " disconnected! (" + reason + ")");
                RemoteClientData(guid);
                using var bsOut = PooledBitStream.GetBitStream();
                bsOut.Write((byte)RisePacketInfo.PLAYER_DESTROY);
                bsOut.Write(guid);
                RakServer.SendToAll(bsOut, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE);
            }
            else
            {
                Debug.Log("[Server] Client " + RakServer.GetAddress(guid, true) + " disconnected! (" + reason + ")");
            }
        }

        void IRakServer.OnReceived(byte packetId, ushort connectionIndex, ulong guid, BitStream bitStream, ulong localTime)
        {
            PlayerData client;
            switch ((RisePacketInfo)packetId)
            {
                case RisePacketInfo.CLIENT_DATA_REPLY:
                    var playerName = bitStream.ReadString();
                    _clients.Add(new PlayerData(guid, playerName));
                    Info.Players.Add(new PlayerAttr { Guid = guid, PlayerName = playerName });
                    using (var bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)RisePacketInfo.CLIENT_DATA_ACCEPTED);
                        Info.Serialize(bsOut);
                        RakServer.SendToClient(bsOut, guid, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
                case RisePacketInfo.PLAYER_MOVE_REQUEST:
                    client = _clients[connectionIndex];
                    var inputPayload = new InputPayload();
                    inputPayload.Deserialize(bitStream);
                    client.PlayerServerController.OnClientInput(inputPayload);
                    break;
                case RisePacketInfo.CLIENT_DATA_REQUEST:
                case RisePacketInfo.CLIENT_DATA_ACCEPTED:
                case RisePacketInfo.PLAYER_MOVED:
                case RisePacketInfo.PLAYER_CONNECTED:
                case RisePacketInfo.PLAYER_DISCONNECTED:
                case RisePacketInfo.PLAYER_SPAWNED:
                    break;
                case RisePacketInfo.PLAYER_SPAWN_REQUEST:
                    client = _clients[connectionIndex];
                    if (client.PlayerServerController != null)
                    {
                        Destroy(client.PlayerServerController.gameObject);
                        using var bsOut = PooledBitStream.GetBitStream();
                        bsOut.Write((byte)RisePacketInfo.PLAYER_DESTROY);
                        bsOut.Write(client.Guid);
                        RakServer.SendToAll(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE);
                    }
                    client.PlayerServerController = Instantiate(_player).GetRequireComponent<PlayerServerController>();
                    client.PlayerServerController.PlayerId = client.Guid;
                    client.PlayerServerController.PlayerIndex = connectionIndex;
                    using (var bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)RisePacketInfo.PLAYER_SPAWNED);
                        bsOut.Write(client.PlayerServerController.transform.position);
                        bsOut.Write(client.Guid);
                        RakServer.SendToAll(bsOut, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
                default: return;
            }
        }
    }
}
