using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Fo76_Communication.Utilities;

namespace Fo76_Communication.SnapshotData.Components
{
    public class ActorCoreSnapshotComponent : Component
    {
        public string Name;
        public uint UiKeyWordFlags;
        public ulong Disabled;
        public bool DeathFade;
        public bool DisableFade;
        public bool Powered;
        public bool InCombat;
        public bool IgnoreCombat;
        public bool Eaten;
        public bool HasDeferredLegendaryDrop;

        public ActorCoreSnapshotComponent(uint EntityId, uint RessourceId, uint ComponentSize, uint ComponentId, byte[] ComponentBuffer) : base(EntityId, RessourceId, ComponentSize, ComponentId, ComponentBuffer)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(this.ComponentBuffer));
            Name = reader.ReadBitMsgString();
            UiKeyWordFlags = reader.ReadUInt32();
            Disabled = (reader.ReadUInt32() | ((ulong)reader.ReadUInt32() << 32));
            DeathFade = reader.ReadBoolean();
            DisableFade = reader.ReadBoolean();
            Powered = reader.ReadBoolean();
            InCombat = reader.ReadBoolean();
            IgnoreCombat = reader.ReadBoolean();
            Eaten = reader.ReadBoolean();
            HasDeferredLegendaryDrop = reader.ReadBoolean();
            
            Console.WriteLine(UiKeyWordFlags);
            Console.WriteLine(reader.BaseStream.Position.ToString("X"));
            Console.ReadLine();

            reader.Close();
        }
    }
}
