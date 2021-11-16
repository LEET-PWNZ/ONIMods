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
        //    public static readonly float[] IntakeCost = new float[]
        //    {
        //        200f,
        //        50f,
        //    };
        //    public static readonly string[] IntakeMats = new string[]
        //{
        //        MATERIALS.REFINED_METALS[0],
        //        SimHashes.Polypropylene.ToString()
        //};
        public static readonly float[] IntakeCost = new float[]
            {
            1f
            };
        public static readonly string[] IntakeMats = new string[]
    {
            MATERIALS.RAW_MINERALS[0]
    };
        public static readonly float[] OutletCost = new float[]
        {
            300f,
            75f,
        };
        public static readonly string[] OutletMats = new string[]
    {
            MATERIALS.REFINED_METALS[0],
            SimHashes.Polypropylene.ToString()
    };
        public static readonly float[] DualPortCost = new float[]
        {
            450f,
            110f,
        };
        public static readonly string[] DualPortMats = new string[]
    {
            MATERIALS.REFINED_METALS[0],
            SimHashes.Polypropylene.ToString()
    };
        public static readonly float[] CompressorCost = new float[]
        {
            1f
        };
        public static readonly string[] CompressorMats = new string[]
        {
            MATERIALS.RAW_MINERALS[0],
        };
        //public static readonly float[] CompressorCost = new float[]
        //{
        //    2000f,
        //    100f,
        //    400f
        //};
        //    public static readonly string[] CompressorMats = new string[]
        //{
        //        MATERIALS.REFINED_METALS[0],
        //        SimHashes.SuperInsulator.ToString(),
        //        MATERIALS.TRANSPARENTS[0]
        //};
    }
}
