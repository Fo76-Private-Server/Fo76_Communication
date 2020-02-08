using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fo76_Communication.Utilities
{
    public static class BinaryReaderExtensions
    {
        public static T FromBinaryReader<T>(this BinaryReader reader)
        {

            // Read in a byte array
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static string ReadBitMsgString(this BinaryReader reader)
        {
            byte[] stringBuffer = new byte[256];

            int stringBufferOffset = 0;
            for (var i = reader.ReadByte(); i > 0; i = reader.ReadByte())
            {
                stringBuffer[stringBufferOffset] = i;
                stringBufferOffset++;
            }

            return Encoding.Default.GetString(stringBuffer);
        }
    }

    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] data)
        {
            string retVar = "";

            if (data == null)
                return retVar;

            foreach (var i in data)
            {
                if (i.ToString("X").Length < 2)
                    retVar = retVar + "0" + i.ToString("X") + " ";
                else
                    retVar = retVar + i.ToString("X") + " ";
            }
            return retVar;
        }
    }
}
