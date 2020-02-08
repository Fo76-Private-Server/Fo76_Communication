using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fo76_Communication
{
    public static class Communication
    {
        public static PacketProcessor Processor { get; private set; }
        public static void Initialize()
        {
            Config.InitializeLogger();
            Processor = new PacketProcessor();
        }
    }
}
