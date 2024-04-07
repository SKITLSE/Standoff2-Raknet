using System.Collections;
using System.Collections.Generic;
using RiseRakNet.RakNet;
using UnityEngine;
using UnityEngine.Networking;

namespace RiseRakNet
{
    public class NetworkInfo : MessageBase
    {
        public List<PlayerAttr> Players = new List<PlayerAttr>();

        public int TickRate;

        public override void Serialize(BitStream writer)
        {
            writer.Write((byte)Players.Count);
            foreach (var item in Players)
            {
                item.Serialize(writer);
            }
            writer.Write(TickRate);
        }

        public override void Deserialize(BitStream reader)
        {
            var count = reader.ReadByte();
            for (var i = 0; i < count; i++)
            {
                var player = new PlayerAttr();
                player.Deserialize(reader);
                Players.Add(player);
            }
            TickRate = reader.ReadInt();
        }
    }
}
