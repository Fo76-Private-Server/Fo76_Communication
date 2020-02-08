using System;
using Fo76_Communication.Utilities;

namespace Fo76_Communication.Packets.Types
{
    public class Message : Packet
    {
        public struct Body
        {
            public byte SequenceNumber;
            public byte FragmentCount;
            public byte FragmentNumber;
            public byte[] MessageData;
        }

        public Body PacketBody;
        public Message(byte[] Data) : base(Data)
        {
            PacketBody = new Body();

            this.PacketBody.SequenceNumber = this.Reader.ReadByte();
            this.PacketBody.FragmentCount = this.Reader.ReadByte();
            if (this.PacketBody.FragmentCount != 0)
            {
                this.PacketBody.FragmentNumber = this.Reader.ReadByte();
            }
            this.PacketBody.MessageData =  new byte[this.Reader.BaseStream.Length - this.Reader.BaseStream.Position];
            this.Reader.Read(this.PacketBody.MessageData, 0, this.PacketBody.MessageData.Length);
        }
    }
}
