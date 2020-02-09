using System;
using System.IO;
using Serilog;
using Fo76_Communication.Packets.Types;
using Fo76_Communication.Utilities;
using Fo76_Communication.SnapshotData;

namespace Fo76_Communication
{
    public class Client
    {
        public ushort SessionId { get; }
        public string Token;

        private bool _fullSequence = true;
        private byte _currentSequenceNumber = 255;
        private byte _currentFragmentCount = 0;
        private byte _currentFragmentNumber = 0;
        private byte[] _sequenceBuffer = new byte[8192];
        private int _currentSequenceBufferOffset = 0;

        public Client(ushort sessionId)
        {
            this.SessionId = sessionId;
        }

        public void ProcessMessage(Message message)
        {
            if (_fullSequence && message.PacketBody.SequenceNumber == (byte) (_currentSequenceNumber + 1))
            {
                _currentSequenceNumber++;
                _currentFragmentCount = message.PacketBody.FragmentCount;
                _currentFragmentNumber = 0;
                _currentSequenceBufferOffset = 0;
                _fullSequence = false;
                Log.Debug(
                    "New sequence from client {@SessionId} with sequence number: {@_currentSequenceNumber} and fragment count: {@_currentFragmentCount}",
                    SessionId, _currentSequenceNumber, _currentFragmentCount);
            }

            if (_fullSequence)
            {
                Log.Information("Old sequence {@_currentSequenceNumber} received. Current sequence {@SequenceNumber}.",
                    _currentSequenceNumber, message.PacketBody.SequenceNumber);
                return;
            }

            if (_currentSequenceNumber != message.PacketBody.SequenceNumber)
            {
                Log.Warning("Invalid sequence number received.");
                return;
            }

            if (_currentFragmentNumber != message.PacketBody.FragmentNumber)
            {
                Log.Warning("Invalid fragment number received.");
                return;
            }

            if (_currentSequenceBufferOffset + message.PacketBody.MessageData.Length > _sequenceBuffer.Length)
            {
                throw (new Exception("Attempted to copy more than " + _sequenceBuffer.Length +
                                     " bytes into the sequenceBuffer"));
            }

            Array.Copy(message.PacketBody.MessageData, 0, _sequenceBuffer, _currentSequenceBufferOffset,
                message.PacketBody.MessageData.Length);
            _currentSequenceBufferOffset += message.PacketBody.MessageData.Length;

            _currentFragmentNumber++;

            PostProcessMessage();
        }

        private void PostProcessMessage()
        {
            if (_currentFragmentNumber != _currentFragmentCount)
            {
                return;
            }

            if (_currentFragmentNumber > _currentFragmentCount)
            {
                throw (new Exception("Current fragment number was bigger than the fragment count"));
            }

            Log.Information(
                "Full sequence from client {@SessionId} received with sequence id {@_currentSequenceNumber}", SessionId,
                _currentSequenceNumber);

            _fullSequence = true;

            Sequence seq = ProcessSequence();

            //handle message 1

            //handle message 2

            //handle snapshot
            if(seq.SnapshotMessageData != null) { 
                Snapshot snapshot = new Snapshot(seq.SnapshotMessageData);
            }
        }

        private Sequence ProcessSequence()
        {
            Sequence seq = new Sequence();
            byte[] data = new byte[this._currentSequenceBufferOffset];
            Array.Copy(_sequenceBuffer, 0, data, 0, data.Length);
            BinaryReader reader = new BinaryReader(new MemoryStream(data));

            try
            {
                seq.Unknown1 = reader.ReadUInt32();
                seq.Unknown2 = reader.ReadByte();

                if ((seq.Unknown2 & 1) != 0)
                {
                    seq.Unknown3 = reader.ReadUInt32();
                    seq.CompressedMessageSize = (ushort) ((reader.ReadUInt16() & 0x7FFF) - 2);
                    seq.UncompressedMessageSize = reader.ReadUInt16();

                    seq.MessageData = new byte[seq.UncompressedMessageSize];
                    byte[] compressedMessageData = new byte[seq.CompressedMessageSize + 0x10];
                    reader.Read(compressedMessageData, 0, seq.CompressedMessageSize);
                    LightweightCompression lWCompression = new LightweightCompression();
                    lWCompression.Start(compressedMessageData, seq.CompressedMessageSize, false);
                    lWCompression.Read(seq.MessageData, seq.UncompressedMessageSize);
                }

                if ((seq.Unknown2 & 2) != 0)
                {
                    seq.Compressed2MessageSize = (ushort) ((reader.ReadUInt16() & 0x7FFF) - 2);
                    seq.Uncompressed2MessageSize = reader.ReadUInt16();

                    seq.Message2Data = new byte[seq.Uncompressed2MessageSize];
                    byte[] compressedMessageData = new byte[seq.Compressed2MessageSize + 0x10];
                    reader.Read(compressedMessageData, 0, seq.Compressed2MessageSize);
                    LightweightCompression lWCompression = new LightweightCompression();
                    lWCompression.Start(compressedMessageData, seq.Compressed2MessageSize, false);
                    lWCompression.Read(seq.Message2Data, seq.Uncompressed2MessageSize);
                }

                seq = ReadSnapshotAcks(reader, seq);

                //client packets dont seem to always contain a snapshot
                if (reader.BaseStream.Position == reader.BaseStream.Length)
                {
                    reader.Close();
                    return seq;
                }

                seq.SnapshotSequence = reader.ReadUInt32();
                seq.BaseSequence = reader.ReadUInt32();
                seq.CompressedBodyMessageSize = reader.ReadUInt32() & 0x7FFFFFFF;
                seq.UncompressedBodyMessageSize = reader.ReadUInt32();
                seq.ComponentCount = reader.ReadUInt16();
                seq.Unknown7 = reader.ReadUInt32();

                byte[] compressedSnapshotData = new byte[seq.CompressedBodyMessageSize + 0x10];
                reader.Read(compressedSnapshotData, 0, (int) seq.CompressedBodyMessageSize);
                seq.SnapshotMessageData = new byte[seq.UncompressedBodyMessageSize];

                LightweightCompression lWSnapCompression = new LightweightCompression();
                lWSnapCompression.Start(compressedSnapshotData, (int) seq.CompressedBodyMessageSize, false);
                lWSnapCompression.Read(seq.SnapshotMessageData, (int) seq.UncompressedBodyMessageSize);

                Log.Debug("Unknown1: {@Unknown1}", seq.Unknown1.ToString("X"));
                Log.Debug("Unknown2: {@Unknown2}", seq.Unknown2);
                Log.Debug("Unknown3: {@Unknown3}", seq.Unknown3);

                Log.Debug("CompressedMessageSize: {@CompressedMessageSize}", seq.CompressedMessageSize);
                Log.Debug("UncompressedMessageSize: {@UncompressedMessageSize}", seq.UncompressedMessageSize);
                Log.Debug("MessageData: {@MessageData}", seq.MessageData);

                Log.Debug("Compressed2MessageSize: {@Compressed2MessageSize}", seq.Compressed2MessageSize);
                Log.Debug("Uncompressed2MessageSize: {@Uncompressed2MessageSize}", seq.Uncompressed2MessageSize);
                Log.Debug("Message2Data: {@Message2Data}", seq.Message2Data);

                Log.Debug("Unknown5: {@Unknown5}", seq.Unknown5);
                Log.Debug("Unknown6: {@Unknown6}", seq.Unknown6);

                Log.Debug("SnapshotSequence: {@SnapshotSequence}", seq.SnapshotSequence);
                Log.Debug("BaseSequence: {@BaseSequence}", seq.BaseSequence);
                Log.Debug("CompressedBodyMessageSize: {@CompressedBodyMessageSize}", seq.CompressedBodyMessageSize);
                Log.Debug("UncompressedBodyMessageSize: {@UncompressedBodyMessageSize}", seq.UncompressedBodyMessageSize);
                Log.Debug("ComponentCount: {@ComponentCount}", seq.ComponentCount);
                Log.Debug("Unknown7: {@Unknown7}", seq.Unknown7);

                if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    throw (new Exception("Didn't read sequence to end"));
                }

                reader.Close();

                return seq;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error("Unknown1: {@Unknown1}", seq.Unknown1.ToString("X"));
                Log.Error("Unknown2: {@Unknown2}", seq.Unknown2);
                Log.Error("Unknown3: {@Unknown3}", seq.Unknown3);

                Log.Error("CompressedMessageSize: {@CompressedMessageSize}", seq.CompressedMessageSize);
                Log.Error("UncompressedMessageSize: {@UncompressedMessageSize}", seq.UncompressedMessageSize);
                Log.Error("MessageData: {@MessageData}", seq.MessageData);

                Log.Error("Compressed2MessageSize: {@Compressed2MessageSize}", seq.Compressed2MessageSize);
                Log.Error("Uncompressed2MessageSize: {@Uncompressed2MessageSize}", seq.Uncompressed2MessageSize);
                Log.Error("Message2Data: {@Message2Data}", seq.Message2Data);

                Log.Error("Unknown5: {@Unknown5}", seq.Unknown5);
                Log.Error("Unknown6: {@Unknown6}", seq.Unknown6);

                Log.Error("SnapshotSequence: {@SnapshotSequence}", seq.SnapshotSequence);
                Log.Error("BaseSequence: {@BaseSequence}", seq.BaseSequence);
                Log.Error("CompressedBodyMessageSize: {@CompressedBodyMessageSize}", seq.CompressedBodyMessageSize);
                Log.Error("UncompressedBodyMessageSize: {@UncompressedBodyMessageSize}", seq.UncompressedBodyMessageSize);
                Log.Error("ComponentCount: {@ComponentCount}", seq.ComponentCount);
                Log.Error("Unknown7: {@Unknown7}", seq.Unknown7);
                Console.ReadLine();

                return new Sequence();
            }
        }

        private Sequence ReadSnapshotAcks(BinaryReader reader, Sequence seq)
        {
            seq.Unknown5 = reader.ReadUInt32();

            var dwLoop = 0;
            do
            {
                seq.Unknown6 |= (byte) (reader.ReadByte() << dwLoop);
                dwLoop += 8;
            } while (dwLoop < 32);

            return seq;
        }
    }
}