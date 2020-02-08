﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Fo76_Communication.Utilities;

namespace Fo76_Communication.Packets.Types
{
    public class PingReply : Packet
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Body
        {
            public uint Timestamp;
            public ushort Bps;
            public ushort Unknown1;
            public byte Unknown2;
        }

        public Body PacketBody;
        public PingReply(byte[] Data) : base(Data)
        {
            this.PacketBody = this.Reader.FromBinaryReader<Body>();
        }
    }
}
