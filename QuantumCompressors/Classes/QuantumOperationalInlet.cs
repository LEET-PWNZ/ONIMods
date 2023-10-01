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
    [AddComponentMenu("KMonoBehaviour/scripts/" + nameof(QuantumOperationalInlet))]
    public class QuantumOperationalInlet:KMonoBehaviour,ISaveLoadable
    {
        [MyCmpReq]
        private Operational _operational;
        private static readonly EventSystem.IntraObjectHandler<QuantumOperationalInlet> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<QuantumOperationalInlet>(((component, mustOperare) => component.OnOperationalChanged((bool)mustOperare)));
        private bool _isDispensing;
        public ConduitType conduitType;
        [SerializeField]
        private float _currentFlow;
        [MyCmpGet]
        protected KBatchedAnimController controller;
        private int _inputCell;
        protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;
        [MyCmpReq]
        public Building building;
        [SerializeField]
        private List<QuantumCompressorComponent> _compressors = new List<QuantumCompressorComponent>();
        [SerializeField]
        private List<Storage> _compressorStorages = new List<Storage>();
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;
        private void OnOperationalChanged(bool mustOperate)
        {
            if (mustOperate)
                _currentFlow = conduitType == ConduitType.Liquid ? 10f : 1f;
            else
                _currentFlow = 0f;
            _operational.SetActive(mustOperate);
        }

        protected void OnMassTransfer(float amount) => _isDispensing = amount > 0f;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            flowAccumulator = Game.Instance.accumulators.Add("Flow", this);
            Subscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
        }

        protected override void OnSpawn()
        {
            OnOperationalChanged(_operational.IsOperational);
            base.OnSpawn();
            _inputCell = building.GetUtilityInputCell();
            Conduit.GetFlowManager(conduitType).AddConduitUpdater(new Action<float>(ConduitUpdate), ConduitFlowPriority.Default);
            UpdateAnim();
            OnCmpEnable();
        }

        private void UpdateCompressors()
        {
            _compressors = _quantumStorageManager.ActiveStorages
                .FindStorage(s => s.GetMyWorldId() == this.GetMyWorldId() 
                && s.conduitType == conduitType 
                && s.GetComponent<Operational>().IsOperational)
                .ToList();
            _compressorStorages = _compressors.Select(c => c.GetComponent<Storage>()).ToList();
        }

        private float RemainingStorageCapacity()
        {
            float result = 0;
            result = _compressorStorages.Sum(s => s.RemainingCapacity());
            return result;
        }

        private void ConduitUpdate(float dt)
        {
            UpdateCompressors();
            ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
            if (!flowManager.HasConduit(_inputCell) || !_compressors.Any())
            {
                OnMassTransfer(0.0f);
                UpdateAnim();
            }
            else
            {
                ConduitFlow.Conduit conduit = flowManager.GetConduit(_inputCell);
                ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);
                float transferInMass = Mathf.Min(contents.mass, _currentFlow * dt);
                float transferOutMass = Mathf.Min(transferInMass, RemainingStorageCapacity());
                if (transferOutMass > 0f)
                {
                    int disease_count = (int)(contents.diseaseCount * (transferOutMass / contents.mass));
                    TransferElement(contents.element, transferOutMass, contents.temperature, contents.diseaseIdx, disease_count, true, false);
                    Game.Instance.accumulators.Accumulate(flowAccumulator, transferOutMass);
                    flowManager.RemoveElement(_inputCell, transferOutMass);
                }
                OnMassTransfer(transferOutMass);
                UpdateAnim();
            }
        }

        void TransferElement(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, bool do_disease_transfer = true)
        {
            foreach (Storage storage in _compressorStorages)
            {
                float remainingCap = storage.RemainingCapacity();
                float transferrableMass = Mathf.Min(mass, remainingCap);
                int transferrableDiseases = mass - transferrableMass == 0 ? disease_count : disease_count * Convert.ToInt32(transferrableMass / mass);
                switch (conduitType)
                {
                    case ConduitType.Gas:
                        storage.AddGasChunk(element, transferrableMass, temperature, disease_idx, transferrableDiseases, keep_zero_mass, do_disease_transfer);
                        break;
                    case ConduitType.Liquid:
                        storage.AddLiquid(element, transferrableMass, temperature, disease_idx, transferrableDiseases, keep_zero_mass, do_disease_transfer);
                        break;
                }
                
                mass -= transferrableMass;
                disease_count -= transferrableDiseases;
                if(mass == 0)
                {
                    break;
                }
            }
        }

        protected override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
            Game.Instance.accumulators.Remove(flowAccumulator);
            Conduit.GetFlowManager(conduitType).RemoveConduitUpdater(new Action<float>(ConduitUpdate));
            base.OnCleanUp();
        }

        public void UpdateAnim()
        {
            if (_operational.IsOperational)
            {
                if (_isDispensing)
                    controller.Queue("on_flow", KAnim.PlayMode.Loop);
                else
                    controller.Queue("on");
            }
            else
                controller.Queue("off");
        }
    }
}
