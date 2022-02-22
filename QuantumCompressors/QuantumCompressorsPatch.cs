using HarmonyLib;
using ONIModsLibrary.Classes;
using QuantumCompressors.BuildingConfigs.Gas;
using QuantumCompressors.BuildingConfigs.Liquid;
using QuantumCompressors.Classes;
using UnityEngine;

namespace QuantumCompressors
{

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class QuantumCompressorsPreLoad
    {
        private static void Prefix() 
        {
            ONIModFunctions.AddStructure("Base", GasQuantumCompressorConfig.ID, GasQuantumCompressorConfig.NAME, GasQuantumCompressorConfig.DESC, GasQuantumCompressorConfig.DESC);
            ONIModFunctions.AddStructure("HVAC", GasCompressorIntakeConfig.ID, GasCompressorIntakeConfig.NAME, GasCompressorIntakeConfig.DESC, GasCompressorIntakeConfig.DESC);
            ONIModFunctions.AddStructure("HVAC", GasCompressorOutletConfig.ID, GasCompressorOutletConfig.NAME, GasCompressorOutletConfig.DESC, GasCompressorOutletConfig.DESC);

            ONIModFunctions.AddStructure("Base", LiquidQuantumCompressorConfig.ID, LiquidQuantumCompressorConfig.NAME, LiquidQuantumCompressorConfig.DESC, LiquidQuantumCompressorConfig.DESC);
            ONIModFunctions.AddStructure("Plumbing", LiquidCompressorIntakeConfig.ID, LiquidCompressorIntakeConfig.NAME, LiquidCompressorIntakeConfig.DESC, LiquidCompressorIntakeConfig.DESC);
            ONIModFunctions.AddStructure("Plumbing", LiquidCompressorOutletConfig.ID, LiquidCompressorOutletConfig.NAME, LiquidCompressorOutletConfig.DESC, LiquidCompressorOutletConfig.DESC);
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
                ONIModFunctions.AddStructureTech(db, "HVAC", GasQuantumCompressorConfig.ID);
                ONIModFunctions.AddStructureTech(db, "HVAC", GasCompressorIntakeConfig.ID);
                ONIModFunctions.AddStructureTech(db, "HVAC", GasCompressorOutletConfig.ID);

                ONIModFunctions.AddStructureTech(db, "LIQUIDTEMPERATURE", LiquidQuantumCompressorConfig.ID);
                ONIModFunctions.AddStructureTech(db, "LIQUIDTEMPERATURE", LiquidCompressorIntakeConfig.ID);
                ONIModFunctions.AddStructureTech(db, "LIQUIDTEMPERATURE", LiquidCompressorOutletConfig.ID);
            }
        }
    }


    //Patch to show filter side screen on QuantumOperationalOutlet
    [HarmonyPatch(typeof(FilterSideScreen), "IsValidForTarget")]
    internal class QuantumElementFilterSideScreen
    {
        static void Postfix(FilterSideScreen __instance, ref bool __result, GameObject target)
        {
            if (!__result)
            {
                __result = !__instance.isLogicFilter && target.GetComponent<QuantumOperationalOutlet>() != null && target.GetComponent<Filterable>() != null;
                return;
            }
        }
    }

    [HarmonyPatch(typeof(BaseUtilityBuildTool), "CheckForConnection")]
    internal class QuantumElementFilterOverrides
    {

        static void Postfix(ref bool __result, int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
        {
            if (!__result)
            {
                outBcv = null;
                Building building = Grid.Objects[cell, 1]?.GetComponent<Building>();
                if (building == null)
                {
                    __result = false;
                    return;
                }
                var compressorComponent = building.GetComponent<QuantumOperationalOutlet>();
                if ((defName.Contains("Liquid") || defName.Contains("Gas")) && compressorComponent != null)
                {
                    var filterCell = compressorComponent.GetFilteredCell();
                    if (cell == filterCell)
                    {
                        outBcv = building.gameObject.GetComponent<BuildingCellVisualizer>();
                        if(outBcv!=null)
                        {
                            if (fireEvents)
                            {
                                outBcv.ConnectedEvent(cell);
                                string sound = GlobalAssets.GetSound(soundName, false);
                                if (sound != null)
                                {
                                    KMonoBehaviour.PlaySound(sound);
                                }
                            }
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}

