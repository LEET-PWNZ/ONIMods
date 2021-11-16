using ONIModsLibrary.Classes;
using QuantumCompressors.BuildingComponents;
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
        public static string PORT_ID = "GasQuantumCompressorLogicPort";
        private const ConduitType conduitType = ConduitType.Gas;
        ONIModConfigManager<QCModConfig> modConf = ONIModConfigManager<QCModConfig>.getInstance();

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "gasstorage_kanim", 100, 120f, QCProperties.CompressorCost, QCProperties.CompressorMats, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = modConf.CurrentConfig.storagePowerConsumption;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.OnePerWorld = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 1), (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
    };
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(GameTags.UniquePerWorld);
            go.AddOrGet<Reservoir>();
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showDescriptor = true;
            defaultStorage.storageFilters = STORAGEFILTERS.GASES;
            defaultStorage.capacityKg = modConf.CurrentConfig.gasStorageCapacityKg;
            defaultStorage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            defaultStorage.showCapacityStatusItem = true;
            defaultStorage.showCapacityAsMainStatus = true;
            go.AddOrGet<SmartReservoir>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }

    }
}
