using ONIModsLibrary.Classes;
using QuantumCompressors.Classes;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumCompressors.BuildingConfigs.Gas
{
    public class GasCompressorOutletConfig : IBuildingConfig
    {
		public const string ID = "GasCompressorOutlet";
		public static string UPPERID { get { return ID.ToUpperInvariant(); } }
		public const string NAME = "Quantum Gas Outlet";
		public static string DESC = "Receives a user-selected " + STRINGS.UI.FormatAsLink("gas", "ELEMENTS_GAS") + " type from a " + STRINGS.UI.FormatAsLink(GasQuantumCompressorConfig.NAME, GasQuantumCompressorConfig.UPPERID) + ".";
		private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 1));

		public override BuildingDef CreateBuildingDef()
		{
            QCModConfig currentConfig = ONIModConfigManager<QCModConfig>.Instance.CurrentConfig;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "valvegas_logic_kanim", 30, 10f,
				currentConfig.outletCost,
				currentConfig.outletMaterials,
				1600f, BuildLocationRule.Anywhere,
				TUNING.BUILDINGS.DECOR.PENALTY.TIER0,
				TUNING.NOISE_POLLUTION.NOISY.TIER1);
			buildingDef.Floodable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = currentConfig.filterPowerConsumption;
			buildingDef.PowerInputOffset = new CellOffset(0, 1);
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			return buildingDef;

		}

		private void AttachPort(GameObject go) => go.AddComponent<ConduitSecondaryOutput>().portInfo = secondaryPort;

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			QuantumOperationalOutlet operationalValve = go.AddOrGet<QuantumOperationalOutlet>();
			operationalValve.portInfo = secondaryPort;
			go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Gas;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			AttachPort(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<RequireInputs>().SetRequirements(true, false);
			go.AddOrGet<LogicOperationalController>().unNetworkedValue = 1;
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		}
	}
}
