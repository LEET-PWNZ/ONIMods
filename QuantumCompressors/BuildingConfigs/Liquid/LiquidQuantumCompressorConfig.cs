using ONIModsLibrary.Classes;
using QuantumCompressors.Classes;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace QuantumCompressors.BuildingConfigs.Liquid
{
    public class LiquidQuantumCompressorConfig : IBuildingConfig
    {
        public static string UPPERID { get { return ID.ToUpperInvariant(); } }
        public const string ID = "LiquidQuantumCompressor";
        public const string NAME = "Liquid Quantum Compressor";
        public static string DESC = "Uses quantum compression to store large amounts of " + UI.FormatAsLink("liquids", "ELEMENTS_LIQUID") + ", and with quantum entanglement, storage management becomes a breeze.";
        private const ConduitType conduitType = ConduitType.Liquid;
        public override BuildingDef CreateBuildingDef()
        {
            QCModConfig currentConfig = ONIModConfigManager<QCModConfig>.Instance.CurrentConfig;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "liquidreservoir_kanim", 100, 120f,
                currentConfig.compressorCost,
                currentConfig.compressorMaterials,
                800f, BuildLocationRule.OnFloor,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1,
                TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = currentConfig.storagePowerConsumption;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
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
            KPrefabID kprefab = go.GetComponent<KPrefabID>();
            kprefab.AddTag(GameTags.NotRocketInteriorBuilding);
            go.AddOrGet<Reservoir>();
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showDescriptor = true;
            defaultStorage.allowItemRemoval = false;
            defaultStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
            defaultStorage.capacityKg = ONIModConfigManager<QCModConfig>.Instance.CurrentConfig.liquidStorageCapacityKg;
            defaultStorage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            defaultStorage.showCapacityStatusItem = true;
            defaultStorage.showCapacityAsMainStatus = true;
            go.AddOrGet<SmartReservoir>();
            QuantumCompressorComponent qcomp = go.AddOrGet<QuantumCompressorComponent>();
            qcomp.conduitType = conduitType;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}
