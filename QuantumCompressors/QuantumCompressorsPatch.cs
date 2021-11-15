using HarmonyLib;
using ONIModsLibrary.Classes;
using QuantumCompressors.BuildingComponents;
using QuantumCompressors.BuildingConfigs.Gas;
using QuantumCompressors.BuildingConfigs.Liquid;

namespace QuantumCompressors
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class QuantumCompressorsPreLoad
    {
        private static void Prefix() 
        {
            ONIModFunctions.AddStructure("Base", GasQuantumCompressorConfig.ID, GasQuantumCompressorConfig.NAME, GasQuantumCompressorConfig.DESC, GasQuantumCompressorConfig.DESC);
            //ONIModFunctions.AddStructure("Base", LiquidQuantumCompressorConfig.ID, LiquidQuantumCompressorConfig.NAME, LiquidQuantumCompressorConfig.DESC, LiquidQuantumCompressorConfig.DESC);
            
            //ONIModFunctions.AddStructure("HVAC", GasCompressorIntakeConfig.ID, GasCompressorIntakeConfig.NAME, GasCompressorIntakeConfig.DESC, GasCompressorIntakeConfig.DESC);
            //ONIModFunctions.AddStructure("HVAC", GasQuantumFilterOutletConfig.ID, GasQuantumFilterOutletConfig.NAME, GasQuantumFilterOutletConfig.DESC, GasQuantumFilterOutletConfig.DESC);
            //ONIModFunctions.AddStructure("HVAC", GasQuantumFilterDualConfig.ID, GasQuantumFilterDualConfig.NAME, GasQuantumFilterDualConfig.DESC, GasQuantumFilterDualConfig.DESC);

            //ONIModFunctions.AddStructure("Plumbing", LiquidCompressorIntakeConfig.ID, LiquidCompressorIntakeConfig.NAME, LiquidCompressorIntakeConfig.DESC, LiquidCompressorIntakeConfig.DESC);
            //ONIModFunctions.AddStructure("Plumbing", LiquidQuantumFilterOutletConfig.ID, LiquidQuantumFilterOutletConfig.NAME, LiquidQuantumFilterOutletConfig.DESC, LiquidQuantumFilterOutletConfig.DESC);
            //ONIModFunctions.AddStructure("Plumbing", LiquidQuantumFilterDualConfig.ID, LiquidQuantumFilterDualConfig.NAME, LiquidQuantumFilterDualConfig.DESC, LiquidQuantumFilterDualConfig.DESC);
        }


    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class QuantumCompressorsDbPostInit
    {
        private static void Postfix()
        {
            var db = Db.Get();
            if (db != null)
            {
                ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", GasQuantumCompressorConfig.ID);
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", LiquidQuantumCompressorConfig.ID);
                
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", GasQuantumFilterOutletConfig.ID);
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", GasCompressorIntakeConfig.ID);
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", GasQuantumFilterDualConfig.ID);

                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", LiquidQuantumFilterOutletConfig.ID);                
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", LiquidCompressorIntakeConfig.ID);
                //ONIModFunctions.AddStructureTech(db, "DupeTrafficControl", LiquidQuantumFilterDualConfig.ID);
            }
        }
    }

    ////Patch to show filter side screen on QuantumElementFilter
    //[HarmonyPatch(typeof(FilterSideScreen), "IsValidForTarget")]
    //internal class QuantumElementFilterSideScreen
    //{
    //    static bool Prefix(FilterSideScreen __instance,ref bool __result, GameObject target)
    //    {

    //        bool isValid;
    //        if (!__instance.isLogicFilter)
    //        {
    //            isValid = (target.GetComponent<QuantumElementFilter>() != null);
    //            if (isValid)
    //            {
    //                __result = isValid && target.GetComponent<Filterable>() != null;
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(BaseUtilityBuildTool), "CheckForConnection")]
    //internal class QuantumElementFilterOverrides
    //{

    //    static bool Prefix(ref bool __result, int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
    //    {
    //        outBcv = null;
    //        GameObject gameObject = Grid.Objects[cell, 1];
    //        Building building = null;
    //        bool isQFilter = false;
    //        if (gameObject != null)
    //        {
    //            building = gameObject.GetComponent<Building>();
    //        }
    //        if (!building) {
    //            __result = false;
    //            return false;
    //        }
    //        int elemFiltOutCellNum = -1;
    //        if (defName.Contains("Liquid") || defName.Contains("Gas"))
    //        {
    //            QuantumElementFilter qElemFiltComp = building.GetComponent<QuantumElementFilter>();
    //            if (qElemFiltComp != null)
    //            {
    //                isQFilter = true;
    //                if (qElemFiltComp.outPortInfo.conduitType == ConduitType.Liquid || qElemFiltComp.outPortInfo.conduitType == ConduitType.Gas)
    //                {
    //                    elemFiltOutCellNum = qElemFiltComp.GetFilteredCell();
    //                }
    //            }
    //        }
    //        if (cell == elemFiltOutCellNum && isQFilter)
    //        {
    //            BuildingCellVisualizer bcvComp = building.gameObject.GetComponent<BuildingCellVisualizer>();
    //            outBcv = bcvComp;
    //            if (bcvComp != null)
    //            {
    //                if (fireEvents)
    //                {
    //                    bcvComp.ConnectedEvent(cell);
    //                    string sound = GlobalAssets.GetSound(soundName, false);
    //                    if (sound != null)
    //                    {
    //                        KMonoBehaviour.PlaySound(sound);
    //                    }
    //                }
    //                __result = true;
    //                return false;
    //            }
    //        }
    //        outBcv = null;
    //        return true;
    //    }
    //}
}

