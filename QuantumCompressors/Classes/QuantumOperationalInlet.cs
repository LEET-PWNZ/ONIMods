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
        [SerializeField]
        public ConduitType conduitType;
        [Serialize]
        private float _currentFlow;
        [MyCmpGet]
        protected KBatchedAnimController controller;
        private int _inputCell;
        protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;
        [MyCmpReq]
        public Building building;
        [SerializeField]
        private QuantumCompressorComponent _compressor;
        [SerializeField]
        private Storage _compressorStorage;
        [SerializeField]
        private Operational _compressorOperational;
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;
        private void OnOperationalChanged(bool mustOperate)
        {
            if (mustOperate)
                _currentFlow = conduitType==ConduitType.Liquid ? 10f : 1f;
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

        private bool HasOperationalStorage()
        {
            bool result = false;
            if (_compressor != null)
            {
                result = _compressorOperational.IsOperational;
            }
            return result;
        }

        private void ensureCompressorExists()
        {
            if (_compressor == null)
            {
                var compressorContextList = _quantumStorageManager.ActiveStorages.FindStorage(s => s.GetMyWorldId() == this.GetMyWorldId() && s.conduitType == conduitType).ToList();
                if (compressorContextList.Count > 0)
                {
                    _compressor = compressorContextList.First();
                    _compressorOperational = _compressor.GetComponent<Operational>();
                    _compressorStorage = _compressor.GetComponent<Storage>();
                }
            }
        }

        private void ConduitUpdate(float dt)
        {
            ensureCompressorExists();
            ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
            if (!flowManager.HasConduit(_inputCell) || !HasOperationalStorage())
            {
                OnMassTransfer(0.0f);
                UpdateAnim();
            }
            else
            {
                ConduitFlow.Conduit conduit = flowManager.GetConduit(_inputCell);
                ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);
                float transferInMass = Mathf.Min(contents.mass, _currentFlow * dt);
                float transferOutMass = Mathf.Min(transferInMass, _compressorStorage.RemainingCapacity());
                if (transferOutMass > 0f)
                {
                    int disease_count = (int)(contents.diseaseCount * (transferOutMass / contents.mass));
                    switch (conduitType)
                    {
                        case ConduitType.Gas:
                            _compressorStorage.AddGasChunk(contents.element, transferOutMass, contents.temperature, contents.diseaseIdx, disease_count, true, false);
                            break;
                        case ConduitType.Liquid:
                            _compressorStorage.AddLiquid(contents.element, transferOutMass, contents.temperature, contents.diseaseIdx, disease_count, true, false);
                            break;
                    }
                    Game.Instance.accumulators.Accumulate(flowAccumulator, transferOutMass);
                    flowManager.RemoveElement(_inputCell, transferOutMass);
                }
                OnMassTransfer(transferOutMass);
                UpdateAnim();
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
