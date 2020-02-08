using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fo76_Communication.Packets.Types
{
    public class Token : Packet
    {
        public Token(byte[] Data) : base(Data)
        {
        }
    }
}
