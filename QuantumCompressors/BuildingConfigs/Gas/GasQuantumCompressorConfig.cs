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

        public override BuildingDef CreateBuildingDef()
        {
            //BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "gasstorage_kanim", 100, 120f, QuantumStorage.CompressorCost, QuantumStorage.CompressorMats, 1000f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0);
            //buildingDef.Floodable = false;
            //buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            //buildingDef.RequiresPowerInput = true;
            //buildingDef.EnergyConsumptionWhenActive = QuantumStorage.QuantumStoragePowerConsume;
            //buildingDef.AudioCategory = "HollowMetal";
            //buildingDef.PowerInputOffset = new CellOffset(1, 0);
            //buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            //buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
            //List<LogicPorts.Port> list = new List<LogicPorts.Port>();
            //list.Add(LogicPorts.Port.OutputPort(PORT_ID, new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE, false, false));
            //buildingDef.LogicOutputPorts = list;
            //GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
            //return buildingDef;

            ONIModConfigManager<QCModConfig> modConf = ONIModConfigManager<QCModConfig>.getInstance();

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "gasstorage_kanim", 100, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.UtilityInputOffset = new CellOffset(1, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
    };
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //go.AddOrGet<Reservoir>();
            ////go.GetComponent<KPrefabID>().AddTag(GameTags.UniquePerWorld, false);
            //Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            //storage.showDescriptor = true;
            //storage.storageFilters = STORAGEFILTERS.GASES;
            //storage.capacityKg = (150f * QuantumStorage.QuantumStorageMultiplier);
            //List<Storage.StoredItemModifier> ReservoirStoredItemModifiers = new List<Storage.StoredItemModifier>();
            //ReservoirStoredItemModifiers.Add(Storage.StoredItemModifier.Hide);
            //ReservoirStoredItemModifiers.Add(Storage.StoredItemModifier.Seal);
            //storage.SetDefaultStoredItemModifiers(ReservoirStoredItemModifiers);
            ////storage.showCapacityStatusItem = true;
            ////storage.showCapacityAsMainStatus = true;
            //go.AddOrGet<SmartReservoir>();
            //var qs=go.AddComponent<QuantumStorage>();
            //qs.conduitType = ConduitType.Gas;

            go.AddOrGet<Reservoir>();
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showDescriptor = true;
            defaultStorage.storageFilters = STORAGEFILTERS.GASES;
            defaultStorage.capacityKg = 150f;
            defaultStorage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            defaultStorage.showCapacityStatusItem = true;
            defaultStorage.showCapacityAsMainStatus = true;
            go.AddOrGet<SmartReservoir>();
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = defaultStorage.capacityKg;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.elementFilter = (SimHashes[])null;
        }

        //public override void DoPostConfigureUnderConstruction(GameObject go)
        //{
        //    //Assets.BuildingDefs.Remove(Assets.GetBuildingDef(ID));
        //    var quc = go.AddComponent<QuantumStorageTracker>();
        //    quc.conduitType = conduitType;
        //}

        public override void DoPostConfigureComplete(GameObject go)
        {
            //var quc = go.AddComponent<QuantumStorageTracker>();
            //quc.conduitType = conduitType;
            ////UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            ////quantumStorageSingleton.gass
            //go.AddOrGetDef<StorageController.Def>();
            //RequireInputs component = go.GetComponent<RequireInputs>();
            //component.SetRequirements(true, false);
            ////go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
            //go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);

            go.AddOrGetDef<StorageController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }

    }
}
