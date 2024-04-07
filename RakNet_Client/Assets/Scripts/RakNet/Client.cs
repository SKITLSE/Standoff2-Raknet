using System.Linq;
using RiseRakNet.Game;
using RiseRakNet.Menu;
using RiseRakNet.Misc;
using RiseRakNet.Player;
using UnityEngine;

namespace RiseRakNet.RakNet
{
    public class Client : LazySingleton<Client>, IRakClient
    {
        private const string Name = "player";
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
                        bsOut.Write(Name);
                        RakClient.Send(bsOut, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE, 0);
                    }
                    break;
                case RisePacketInfo.CLIENT_DATA_ACCEPTED:
                    NetworkInfo.Deserialize(bitStream);
                    _playerAttr = NetworkInfo.Players.First(a => a.Guid == RakClient.Guid);
                    _status = null;
                    Game.Game.Init("TestLevel", _playerAttr);
                    break;
                case RisePacketInfo.PLAYER_SPAWNED:
                    var position = bitStream.ReadVector3();
                    var id = bitStream.ReadULong();
                    GameController.Instance.OnPlayerSpawn(id, position);
                    break;
                case RisePacketInfo.PLAYER_MOVED:
                    var statePayload = new StatePayload();
                    var playerId = bitStream.ReadULong();
                    statePayload.Deserialize(bitStream);
                    GameController.Instance.OnPlayerMoved(playerId, statePayload);
                    break;
                case RisePacketInfo.PLAYER_DESTROY:
                    GameController.Instance.OnPlayerRemoved(bitStream.ReadULong());
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
