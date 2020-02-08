using System;
using System.IO;
using System.Runtime.InteropServices;
using Fo76_Communication.Packets.Types;
using Fo76_Communication.Utilities;

namespace Fo76_Communication.Packets
{
    //should bit test instead to differentiate
    public enum PacketType : byte
    {
        Token = 0,
        PingRequest = 0x80,
        PingReply = 0x81,
        Message = 0x82
    }

    public abstract class Packet
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketHead
        {
            public ushort Session;
            public PacketType Type;
        }

        public PacketHead Head;
        public BinaryReader Reader;

        protected Packet(byte[] Data)
        {
            this.Reader = new BinaryReader(new MemoryStream(Data));
            this.Head = this.Reader.FromBinaryReader<PacketHead>();
        }

        public static Packet ParsePacket(byte[] Data)
        {
            switch ((PacketType)Data[2])
            {
                case PacketType.Message:
                    return new Message(Data);
                case PacketType.PingReply:
                    return new PingReply(Data);
                case PacketType.PingRequest:
                    return new PingRequest(Data);
                case PacketType.Token:
                    return new Token(Data);
                default:
                    throw (new Exception("Unknown Packet"));
            }
        }
    }
}
