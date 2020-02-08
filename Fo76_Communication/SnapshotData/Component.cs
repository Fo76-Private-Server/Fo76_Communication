using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fo76_Communication.SnapshotData.Components;

namespace Fo76_Communication.SnapshotData
{
    //seems to still be missing a few components
    public enum ComponentType : uint
    {
        ActorCoreSnapshotComponent = 1,
        TESObjectREFRCoreComponent = 2,
        TESObjectREFRScriptObjectSnapshotComponent = 3,
        ActorValueSnapshotComponent = 4,
        DestructibleObjectSnapshotComponent = 10,
        ActorPerksSnapshotComponent = 11,
        TESObjectREFRPhysicsSnapshotComponent = 12,
        ProjectilePhysicsSnapshotComponent = 74,
        RemoteServerActorBehaviorComponent = 14,
        WorkshopNetworkSnapshotComponent = 15,
        PlayerCoreSnapshotComponent = 16,
        ExtraPowerLinksSnapshotComponent = 18,
        SnapshotComponentBGSBendableSpline = 19,
        ActorValuesSnapshotComponent = 20,
        TESObjectREFRTransformSnapshotComponent = 21,
        ActorPathComponent = 25,
        RemoteServerActorMotionComponent = 28,
        ActorPowerArmorWorkbenchComponent = 30,
        RadioSoundComponent = 64,
        RadioFileComponent = 65,
        PlayerPrivateCoreSnapshotComponent = 66,
        RemoteServerActorProjectileComponent = 68,
        SnapshotComponentExtraPowerArmorOwnership = 69,
        RadioReceiverComponent = 70,
        AnimationProgressComponent = 71,
        ExtraDataCoreSnapshotComponent = 72,
        ExtraKeypadDataSnapshotComponent = 73,
        TESObjectREFRQuestItemSnapshotComponent = 75,
        ClientPlayerCoreSnapshotComponent = 79,
        QuickPlayBabylonDataComponent = 177,
        SnapshotComponentExtraPowerArmor = 82,
        SnapshotComponentExtraPlayerStorage = 105,

        ClientInventoryItemComponent = 233, //0xE9 - 0xFB //14000000013329E2
        ClientInventoryResourceComponent = 252, //0xFC - 0xFE

        UNKNOWN = 255
    }

    public class Component
    {
        public uint EntityId;
        public uint RessourceId;
        public uint ComponentSize;
        public uint ComponentId;
        public ComponentType Type;
        public byte[] ComponentBuffer;

        protected Component(uint EntityId, uint RessourceId, uint ComponentSize, uint ComponentId, byte[] ComponentBuffer)
        {
            this.EntityId = EntityId;
            this.RessourceId = RessourceId;
            this.ComponentSize = ComponentSize;
            this.ComponentId = ComponentId;
            this.Type = GetComponentType(this.ComponentId);
            this.ComponentBuffer = ComponentBuffer;
        }

        public static Component GetComponent(uint EntityId, uint RessourceId, uint ComponentSize, uint ComponentId, byte[] ComponentBuffer)
        {
            ComponentType type = GetComponentType(ComponentId);
            switch (type)
            {
                case ComponentType.ActorCoreSnapshotComponent: 
                    return new ActorCoreSnapshotComponent(EntityId, RessourceId, ComponentSize, ComponentId, ComponentBuffer);
            }

            return new Component(EntityId, RessourceId, ComponentSize, ComponentId, ComponentBuffer);
        }

        public static ComponentType GetComponentType(uint ComponentId)
        {
            if (Enum.IsDefined(typeof(ComponentType), ComponentId))
            {
                return (ComponentType)ComponentId;
            }

            if (ComponentId >= 0xE9 && ComponentId <= 0xFB)
            {
                return ComponentType.ClientInventoryItemComponent;
            }

            if (ComponentId >= 0xFC && ComponentId <= 0xFE)
            {
                return ComponentType.ClientInventoryResourceComponent;
            }

            return ComponentType.UNKNOWN;
        }

        public override string ToString()
        {
            return "EntityId:" + EntityId.ToString("X") + " RessourceId:" + RessourceId.ToString("X") + " ComponentSize:" + ComponentSize.ToString("X") + " ComponentId:" + ComponentId.ToString("X") + " Type:" + Type;
        }
    }
}