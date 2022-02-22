using ONIModsLibrary.Classes;
using QuantumCompressors.Classes;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace QuantumCompressors.BuildingConfigs.Gas
{

    public class GasQuantumCompressorConfig : IBuildingConfig
    {
        public static string UPPERID { get { return ID.ToUpperInvariant(); } }
        public const string ID = "GasQuantumCompressor";
        public const string NAME = "Gas Quantum Compressor";
        public static string DESC = "Uses quantum compression to store large amounts of " + UI.FormatAsLink("gases", "ELEMENTS_GAS") + ", and with quantum entanglement, storage management becomes a breeze.";
        private const ConduitType conduitType = ConduitType.Gas;
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "gasstorage_kanim", 100, 120f,
                QCProperties.CompressorCost,
                QCProperties.CompressorMaterials,
                800f, BuildLocationRule.OnFloor,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1,
                TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = ONIModConfigManager<QCModConfig>.Instance.CurrentConfig.storagePowerConsumption;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.OnePerWorld = true;
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort( SmartReservoir.PORT_ID,
                new CellOffset(0, 0),
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT,
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE,
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
            };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            var kprefab = go.GetComponent<KPrefabID>();
            kprefab.AddTag(GameTags.UniquePerWorld);
            kprefab.AddTag(GameTags.NotRocketInteriorBuilding);
            go.AddOrGet<Reservoir>();
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showDescriptor = true;
            defaultStorage.storageFilters = STORAGEFILTERS.GASES;
            defaultStorage.capacityKg = ONIModConfigManager<QCModConfig>.Instance.CurrentConfig.gasStorageCapacityKg;
            defaultStorage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            defaultStorage.showCapacityStatusItem = true;
            defaultStorage.showCapacityAsMainStatus = true;
            go.AddOrGet<SmartReservoir>();
            var qcomp = go.AddOrGet<QuantumCompressorComponent>();
            qcomp.conduitType = conduitType;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }

    }
}
