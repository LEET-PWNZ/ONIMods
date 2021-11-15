using STRINGS;
using System.Linq;

namespace ONIModsLibrary.Classes
{
    public class ONIModFunctions
    {
        public static void AddStructure(string structureCategory, string structureId, string structureName, string structureDesc, string structureEffect)
        {
            string upperCaseID = structureId.ToUpperInvariant();
            Strings.Add(new string[] { "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".NAME", UI.FormatAsLink(structureName, structureId) });
            Strings.Add(new string[] { "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".DESC", structureDesc });
            Strings.Add(new string[] { "STRINGS.BUILDINGS.PREFABS." + upperCaseID + ".EFFECT", structureEffect });
            ModUtil.AddBuildingToPlanScreen(structureCategory, structureId);

        }

        public static void AddStructureTech(Db db, string techCategory, string structureId)
        {
            var tec = db.Techs.resources.Where(t => t.Id == techCategory).FirstOrDefault();
            if (tec != null)
            {
                tec.unlockedItemIDs.Add(structureId);
            }

        }
    }
}
