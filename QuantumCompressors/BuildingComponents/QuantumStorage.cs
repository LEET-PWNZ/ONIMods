using KSerialization;
using QuantumCompressors.BuildingConfigs;
using QuantumCompressors.BuildingConfigs.Gas;
using QuantumCompressors.BuildingConfigs.Liquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace QuantumCompressors.BuildingComponents
{
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/scripts/"+nameof(QuantumStorage))]
    public class QuantumStorage:KMonoBehaviour, ISaveLoadable
    {
        
        public ConduitType conduitType;
        [MyCmpReq]
        public Storage storage;
        [MyCmpReq]
        public Operational operational;
        private QuantumStorageItem itemConfig;
        protected override void OnCleanUp()
        {
            //BuildingComplete com = GetComponent<BuildingComplete>();
            QuantumStorageSingleton quantumStorage = QuantumStorageSingleton.Get();
            quantumStorage.StorageItems.Remove(itemConfig);
            base.OnCleanUp();
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            //BuildingComplete com = GetComponent<BuildingComplete>();
            itemConfig = new QuantumStorageItem
            {
                //worldId = com.GetMyWorldId(),
                conduitType = conduitType,
                operational = operational,
                storage = storage
            };
            QuantumStorageSingleton quantumStorage = QuantumStorageSingleton.Get();
            quantumStorage.StorageItems.Add(itemConfig);

        }
    }
}
