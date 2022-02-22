using KSerialization;
using QuantumCompressors.BuildingConfigs.Gas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumCompressors.Classes
{

    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/scripts/" + nameof(QuantumCompressorComponent))]
    public class QuantumCompressorComponent : KMonoBehaviour,ISaveLoadable
    {
        [SerializeField]
        public ConduitType conduitType;
        [MyCmpReq]
        private Operational _operational;
        private static readonly EventSystem.IntraObjectHandler<QuantumCompressorComponent> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<QuantumCompressorComponent>(((component, mustOperare) => component.OnOperationalChanged((bool)mustOperare)));
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
        }

        protected override void OnSpawn()
        {
            OnOperationalChanged(_operational.IsOperational);
            base.OnSpawn();
            if (!_quantumStorageManager.ActiveStorages.Contains(this)) _quantumStorageManager.ActiveStorages.Add(this);
            OnCmpEnable();
        }

        protected override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
            if (_quantumStorageManager.ActiveStorages.Contains(this)) _quantumStorageManager.ActiveStorages.Remove(this);
            base.OnCleanUp();
        }

        private void OnOperationalChanged(bool mustOperate)
        {
            _operational.SetActive(mustOperate);
        }

    }

}
