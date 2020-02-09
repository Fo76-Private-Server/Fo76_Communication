using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fo76_Communication;

namespace Fo76_Communication_Tests
{
    class Program
    {
        enum MessageDirection
        {
            Sent,
            Received,
            Unknown
        }

        static void Main(string[] args)
        {
            Communication.Initialize();

            /*Communication.Processor.ProcessPacket(TestData.stubFirstMessage);
            Communication.Processor.ProcessPacket(TestData.stubSecondMessage);
            Communication.Processor.ProcessPacket(TestData.stubThirdMessage);
            Communication.Processor.ProcessPacket(TestData.stubFourthMessage);
            Console.WriteLine("Done");
            Console.ReadLine();*/

            string[] logLines = File.ReadAllLines("fo76_character_creation.log");
            int linesParsed = 0;

            foreach (var line in logLines)
            {
                MessageDirection messageDirection = MessageDirection.Unknown;
                if (line.StartsWith("sendEncrypted"))
                {
                    messageDirection = MessageDirection.Sent;
                }

                if (line.StartsWith("recvEncrypted"))
                {
                    messageDirection = MessageDirection.Received;
                }

                if (messageDirection == MessageDirection.Unknown)
                {
                    throw (new Exception("Unable to parse Line:" + line));
                }

                if (messageDirection == MessageDirection.Received)
                {
                    linesParsed++;
                    continue;
                }

                string lineHexString = line.Split('-')[1].Remove(0, 1);
                byte[] lineData = StringToByteArray(lineHexString);

                Communication.Processor.ProcessPacket(lineData);
                linesParsed++;
            }

            Console.WriteLine("Done. Parsed " + linesParsed + "/" + logLines.Length + " lines.");
            Console.ReadLine();
        }

        //https://stackoverflow.com/a/321404
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
