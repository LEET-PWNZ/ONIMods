using ONIModsLibrary.Classes;
using QuantumCompressors.BuildingComponents;
using QuantumCompressors.Classes.QuantumInputValve;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace QuantumCompressors.BuildingConfigs.Gas
{
    public class GasCompressorIntakeConfig : IBuildingConfig
    {

		public const string ID = "GasCompressorIntake";
		public static string UPPERID { get { return ID.ToUpperInvariant(); } }
		public const string NAME = "Quantum Gas Intake";
		public static string DESC = "Receives "+UI.FormatAsLink("gas", "ELEMENTS_GAS") +" and entangles it into a "+UI.FormatAsLink(GasQuantumCompressorConfig.NAME, GasQuantumCompressorConfig.UPPERID)+".";
		private ConduitPortInfo inputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));
		ONIModConfigManager<QCModConfig> modConf = ONIModConfigManager<QCModConfig>.getInstance();

		public override BuildingDef CreateBuildingDef()
        {
			
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "valvegas_logic_kanim", 30, 10f, QCProperties.IntakeCost, QCProperties.IntakeMats,
				1600f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, TUNING.NOISE_POLLUTION.NOISY.TIER1, 0.2f);
			buildingDef.InputConduitType = inputPort.conduitType;
			buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = modConf.CurrentConfig.portPowerConsumption;
            buildingDef.PowerInputOffset =new CellOffset(0,1);
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.UtilityInputOffset= inputPort.offset;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			return buildingDef;

		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			QuantumOperationalInputValve operationalValve = go.AddOrGet<QuantumOperationalInputValve>();
            operationalValve.conduitType = ConduitType.Gas;
            operationalValve.maxFlow = 1f;
        }

		public override void DoPostConfigureComplete(GameObject go)
		{
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
			go.GetComponent<RequireInputs>().SetRequirements(true, false);
			go.AddOrGet<LogicOperationalController>().unNetworkedValue = 1;
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		}

    }
}
