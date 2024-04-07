using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEngine;


namespace RiseRakNet
{
    public class Client : LazySingleton<Client>, IRakClient
    {
        private string _name = "player";
        private InfoStatus _status;
        private PlayerAttr _playerAttr = new();
        public NetworkInfo NetworkInfo = new();
        private void Awake()
        {
            DontDestroyOnLoad(this);
            RakClient.RegisterInterface(this);
        }

        public void Connect(string ip, ushort port, InfoStatus status)
        {
            _status = status;
            RakClient.Connect(ip, port);
        }

        public void OnConnected(string address, ushort port, string password)
        {
            Debug.Log("[SampleClient] Connected to " + address + ":" + port);
            if (_status != null) _status.Info.text = "Connected!";
            _status = null;
            Game.Init("TestLevel", _playerAttr);
        }

        public void OnConnecting(string address, ushort port, string password)
        {
            Debug.Log("[SampleClient] Connecting to " + address + ":" + port);
            if (_status != null) _status.Info.text = "Connecting!";
        }

        public void OnDisconnected(DisconnectReason reason, string message = "")
        {
            Debug.Log("[SampleClient] Disconnected " + reason + " -> " + message);
            if (_status != null) _status.Info.text = $"Disconnected {reason} -> {message}";
        }

        public void OnReceived(byte packetId, uint packetSize, BitStream bitStream, ulong localTime)
        {
            switch ((RisePacketInfo)packetId)
            {
                case RisePacketInfo.CLIENT_DATA_REQUEST:
                    using (var bsOut = PooledBitStream.GetBitStream())
                    {
                        bsOut.Write((byte)RisePacketInfo.CLIENT_DATA_REPLY);
                        bsOut.Write(_name);
                        RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
                case RisePacketInfo.CLIENT_DATA_ACCEPTED:
                    break;
                case RisePacketInfo.PLAYER_SPAWNED:
                    var position = bitStream.ReadVector3();
                    var id = bitStream.ReadULong();
                    GameController.Instance.OnPlayerSpawn(id, position);
                    break;
                case RisePacketInfo.PLAYER_MOVED:
                    var player = bitStream.ReadULong();
                    var prevPos = bitStream.ReadVector3();
                    var move = bitStream.ReadVector3();
                    GameController.Instance.OnPlayerMoved(player, move, prevPos);
                    break;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            RakClient.UnRegisterInterface(this);
        }
    }
    public enum RisePacketInfo
    {
        CLIENT_DATA_REQUEST = InternalPacketID.ID_USER_PACKET_ENUM,
        CLIENT_DATA_REPLY,
        CLIENT_DATA_ACCEPTED,
        PLAYER_MOVE_REQUEST,
        PLAYER_MOVED,
        PLAYER_CONNECTED,
        PLAYER_DISCONNECTED,
        PLAYER_SPAWN_REQUEST,
        PLAYER_SPAWNED,
        PLAYER_DESTROY
    }
}
