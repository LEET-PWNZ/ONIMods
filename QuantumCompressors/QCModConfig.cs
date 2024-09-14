using ONIModsLibrary.Classes;

namespace QuantumCompressors
{
    public class QCModConfig : ONIModConfigBase<QCModConfig>
    {
        public override QCModConfig GetDefaultConfig()
        {
            return new QCModConfig {
                gasStorageCapacityKg = 20000f,
                liquidStorageCapacityKg = 100000f,
                storagePowerConsumption = 240f,
                portPowerConsumption = 10f,
                filterPowerConsumption = 15f,
                intakeCost = new float[] { 20f, 20f },
                intakeMaterials = new string[] { "RefinedMetal", "Polypropylene" },
                outletCost = new float[] { 30f, 30f },
                outletMaterials = new string[] { "RefinedMetal", "Polypropylene"},
                compressorCost = new float[] { 200f, 50f, 50f },
                compressorMaterials = new string[] { "RefinedMetal", "Polypropylene", "Transparent" }
            };
        }

        public float gasStorageCapacityKg { get; set; }
        public float liquidStorageCapacityKg { get; set; }
        public float storagePowerConsumption { get; set; }
        public float portPowerConsumption { get; set; }
        public float filterPowerConsumption { get; set; }
        public float[] intakeCost { get; set; }
        public string[] intakeMaterials { get; set; }
        public float[] outletCost { get; set; }
        public string[] outletMaterials { get; set; }
        public float[] compressorCost { get; set; }
        public string[] compressorMaterials { get; set; }


    }
}
