using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fo76_Communication
{
    public struct Sequence
    {
        //Sequence information
        public uint Unknown1;
        public byte Unknown2;
        public uint Unknown3;

        //Message 1
        public ushort CompressedMessageSize;
        public ushort UncompressedMessageSize; //seems to be something like uncompressed message size but drops a ton of bytes
        public byte[] MessageData;

        //Message 2
        public ushort Compressed2MessageSize;
        public ushort Uncompressed2MessageSize; //same as above
        public byte[] Message2Data;

        //Snapshot Acks
        public uint Unknown5;
        public byte Unknown6;

        //Snapshot
        public uint SnapshotSequence;
        public uint BaseSequence;
        public uint CompressedBodyMessageSize;
        public uint UncompressedBodyMessageSize;
        public ushort ComponentCount;
        public uint Unknown7;
        public byte[] SnapshotMessageData;
    }
}
