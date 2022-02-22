using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace QuantumCompressors
{
    public class QCProperties
    {
        
        public static readonly float[] IntakeCost = { 200f, 50f };
        public static readonly string[] IntakeMaterials = { MATERIALS.REFINED_METALS[0], SimHashes.Polypropylene.ToString() };
        public static readonly float[] OutletCost = { 300f, 75f };
        public static readonly string[] OutletMaterials = { MATERIALS.REFINED_METALS[0], SimHashes.Polypropylene.ToString() };
        public static readonly float[] CompressorCost = { 2000f, 100f, 400f };
        public static readonly string[] CompressorMaterials = { MATERIALS.REFINED_METALS[0], SimHashes.SuperInsulator.ToString(), MATERIALS.TRANSPARENTS[0] };
    }
}
