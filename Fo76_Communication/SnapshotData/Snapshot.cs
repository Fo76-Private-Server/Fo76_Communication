using Fo76_Communication.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Fo76_Communication.SnapshotData
{
    public class Snapshot
    {
        public List<Component> Components = new List<Component>();
        public Snapshot(byte[] data)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                Component component = ParseComponent(reader);
                this.Components.Add(component);
            }
            reader.Close();
        }

        //cleanup needed
        private Component ParseComponent(BinaryReader reader)
        {
            byte v278 = reader.ReadByte();
            uint v30 = (uint) (v278 >> 6);
            uint v31 = (uint) (((v278 >> 4) & 3) + 1);
            uint v32 = 0;
            uint v33 = 0;
            uint v34 = 0;
            if (v30 != 3)
            {
                v32 = (uint) ((v278 & 8) > 0 ? 1 : v32);
                v32++;
            }

            if (v30 == 3 || v30 != 0)
            {
                v33 = 0;
                v34 = 0;
            }
            else
            {
                v33 = (((uint) (byte) v278 >> 1) & 3) + 1;
            }

            v34 = 1;

            if ((v30 == 3 || v30 != 0) && v30 != 1)
            {
                v34 = 0;
            }

            uint EntityId = 0;
            uint ComponentId = 0;
            uint RessourceId = 0;
            uint ComponentSize = 0;
            bool UseZeroRunLengthCompression = ((v278 & 1) == 0); //v104

            int v28 = 0;
            if (v31 != 0)
            {
                do
                {
                    EntityId |= (uint) (reader.ReadByte() << v28);
                    v28 += 8;
                    --v31;
                } while (v31 != 0);

                v28 = 0;
            }

            if (v32 != 0)
            {
                uint v37 = v32;
                do
                {
                    ComponentId |= (uint) (reader.ReadByte() << v28);
                    v28 += 8;
                    --v37;
                } while (v37 != 0);
            }

            if (v33 != 0)
            {
                int v39 = 0;
                uint v40 = v33;
                do
                {
                    RessourceId |= (uint) (reader.ReadByte() << v39);
                    v39 += 8;
                    --v40;
                } while (v40 != 0);
            }

            if (v34 != 0)
            {
                int v41 = 0;
                uint v42 = v34;
                do
                {
                    ComponentSize |= (uint) (reader.ReadByte() << v41);
                    v41 += 8;
                    --v42;
                } while (v42 != 0);
            }

            byte[] componentBuffer;
            if (UseZeroRunLengthCompression)
            {
                ZeroRunLengthCompression comp = new ZeroRunLengthCompression();
                componentBuffer = new byte[ComponentSize];
                comp.Start(new MemoryStream(componentBuffer), (MemoryStream) reader.BaseStream,
                    componentBuffer.Length);
                comp.ReadBytes(componentBuffer, componentBuffer.Length);
            }
            else
            {
                componentBuffer = new byte[ComponentSize];
                reader.Read(componentBuffer, 0, componentBuffer.Length);
            }

            return Component.GetComponent(EntityId, RessourceId, ComponentSize, ComponentId, componentBuffer);
        }
    }
}