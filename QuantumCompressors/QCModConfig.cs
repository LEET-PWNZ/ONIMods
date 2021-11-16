using ONIModsLibrary.Classes;

namespace QuantumCompressors
{
    public class QCModConfig : ONIModConfigBase<QCModConfig>
    {
        public override QCModConfig GetDefaultConfig()
        {
            return new QCModConfig {
                gasStorageCapacityKg = 9000f,
                liquidStorageCapacityKg = 300000f,
                storagePowerConsumption = 2000f,
                portPowerConsumption = 10f,
                filterPowerConsumption=20f
            };
        }

        public float gasStorageCapacityKg { get; set; }
        public float liquidStorageCapacityKg { get; set; }
        public float storagePowerConsumption { get; set; }
        public float portPowerConsumption { get; set; }
        public float filterPowerConsumption { get; set; }
        public string testProp { get; set; }

    }
}
