namespace RiseRakNet.RakNet
{
    public abstract class MessageBase
    {
        public virtual void Serialize(BitStream stream) { }
        public virtual void Deserialize(BitStream stream) { }
    }
}