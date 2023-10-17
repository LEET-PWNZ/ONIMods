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
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;
        private int _worldId;
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
            _worldId = this.GetMyWorldId();
            _inputCell = building.GetUtilityInputCell();
            Conduit.GetFlowManager(conduitType).AddConduitUpdater(new Action<float>(ConduitUpdate), ConduitFlowPriority.Default);
            UpdateAnim();
            OnCmpEnable();
        }

        private void UpdateCompressors()
        {
            _compressors = _quantumStorageManager
                .FindStorage(conduitType, _worldId);
        }

        private float RemainingStorageCapacity(List<QuantumCompressorComponent> usableCompressors)
        {
            float result = 0;
            result = usableCompressors.Sum(s => s.RemainingCapacity());
            return result;
        }

        private void ConduitUpdate(float dt)
        {
            UpdateCompressors();
            ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
            bool activeFlag = false;
            if (!flowManager.HasConduit(_inputCell) || !_compressors.Any())
            {
                OnMassTransfer(0.0f);
                UpdateAnim();
            }
            else
            {
                ConduitFlow.Conduit conduit = flowManager.GetConduit(_inputCell);
                ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);

                List<QuantumCompressorComponent> usableCompressorListOrdered = _compressors
                .Where(c => c.GetSelectedTag == contents.element.CreateTag())
                .Union(_compressors
                .Where(c => c.GetSelectedTag.Name.Equals("void", StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

                float transferInMass = Mathf.Min(contents.mass, _currentFlow * dt);
                float transferOutMass = Mathf.Min(transferInMass, RemainingStorageCapacity(usableCompressorListOrdered));
                if (transferOutMass > 0f)
                {
                    activeFlag = true;
                    int disease_count = (int)(contents.diseaseCount * (transferOutMass / contents.mass));
                    TransferElement(contents.element, transferOutMass, contents.temperature, contents.diseaseIdx, disease_count, true, usableCompressorListOrdered, false);
                    Game.Instance.accumulators.Accumulate(flowAccumulator, transferOutMass);
                    flowManager.RemoveElement(_inputCell, transferOutMass);
                }
                OnMassTransfer(transferOutMass);
                UpdateAnim();
            }
            _operational.SetActive(activeFlag);
        }

        void TransferElement(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, List<QuantumCompressorComponent> usableCompressors, bool do_disease_transfer = true)
        {
            
            foreach (QuantumCompressorComponent compressor in usableCompressors)
            {
                float remainingCap = compressor.RemainingCapacity();
                float transferrableMass = Mathf.Min(mass, remainingCap);
                int transferrableDiseases = mass - transferrableMass == 0 ? disease_count : disease_count * Convert.ToInt32(transferrableMass / mass);
                compressor.AddElementChunk(element, transferrableMass, temperature, disease_idx, transferrableDiseases, keep_zero_mass, do_disease_transfer);
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
