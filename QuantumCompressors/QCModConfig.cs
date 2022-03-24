using ONIModsLibrary.Classes;

namespace QuantumCompressors
{
    public class QCModConfig : ONIModConfigBase<QCModConfig>
    {
        public override QCModConfig GetDefaultConfig()
        {
            return new QCModConfig {
                gasStorageCapacityKg = 9000f,
                liquidStorageCapacityKg = 100000f, //Game base storage is limited to 100 ton, hoping to find a way around this in a future version
                storagePowerConsumption = 960f,
                portPowerConsumption = 30f,
                filterPowerConsumption=60f,
                intakeCost =new float[] { 50f, 25f },
                intakeMaterials=new string[] { "RefinedMetal", "Polypropylene" },
                outletCost =new float[] { 100f, 50f },
                outletMaterials=new string[] { "RefinedMetal", "Polypropylene"},
                compressorCost=new float[] { 800f,400f, 400f },
                compressorMaterials= new string[] { "RefinedMetal", "Polypropylene", "Transparent" }
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
