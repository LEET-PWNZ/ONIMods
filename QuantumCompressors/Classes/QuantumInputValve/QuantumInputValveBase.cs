using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumCompressors.Classes.QuantumInputValve
{
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/scripts/"+nameof(QuantumInputValveBase))]
    public class QuantumInputValveBase : KMonoBehaviour, ISaveLoadable
    {
        [SerializeField]
        public ConduitType conduitType;
        [SerializeField]
        public float maxFlow = 0.5f;
        [Serialize]
        private float currentFlow;
        [MyCmpGet]
        protected KBatchedAnimController controller;
        protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;
        private int curFlowIdx = -1;
        private int inputCell;
        //remo
        //private int outputCell;
        [SerializeField]
        public ValveBase.AnimRangeInfo[] animFlowRanges;

        public float CurrentFlow
        {
            set => currentFlow = value;
            get => currentFlow;
        }
        public float MaxFlow => maxFlow;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            flowAccumulator = Game.Instance.accumulators.Add("Flow", this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Building component = GetComponent<Building>();
            inputCell = component.GetUtilityInputCell();
            //TODO init output to tank
            //outputCell = component.GetUtilityOutputCell();
            Conduit.GetFlowManager(conduitType).AddConduitUpdater(new Action<float>(ConduitUpdate), ConduitFlowPriority.Default);
            UpdateAnim();
            OnCmpEnable();
        }

        protected override void OnCleanUp()
        {
            Game.Instance.accumulators.Remove(flowAccumulator);
            Conduit.GetFlowManager(conduitType).RemoveConduitUpdater(new Action<float>(ConduitUpdate));
            base.OnCleanUp();
        }

        private void ConduitUpdate(float dt)
        {
            ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
            ConduitFlow.Conduit conduit = flowManager.GetConduit(inputCell);
            if (!flowManager.HasConduit(inputCell)) //TODO if tank not avail || !flowManager.HasConduit(outputCell)
            {
                OnMassTransfer(0.0f);
                UpdateAnim();
            }
            else
            {
                ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);
                float mass = Mathf.Min(contents.mass, currentFlow * dt);
                float transferredElement = 0.0f;
                if (mass > 0f)
                {
                    int disease_count = (int)(mass / contents.mass * ((float)contents.diseaseCount));
                    //TODO transfer actual contents
                    //transferredElement = flowManager.AddElement(outputCell, contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count);
                    Game.Instance.accumulators.Accumulate(flowAccumulator, transferredElement);
                    if (transferredElement > 0f)
                        flowManager.RemoveElement(inputCell, transferredElement);
                }
                OnMassTransfer(transferredElement);
                UpdateAnim();
            }
        }

        protected virtual void OnMassTransfer(float amount)
        {
        }

        public virtual void UpdateAnim()
        {
            float averageRate = Game.Instance.accumulators.GetAverageRate(flowAccumulator);
            if (averageRate > 0f)
            {
                for (int index = 0; index < animFlowRanges.Length; index++)
                {
                    if (averageRate <= animFlowRanges[index].minFlow)
                    {
                        if (curFlowIdx == index)break;
                        curFlowIdx = index;
                        controller.Play(animFlowRanges[index].animName, averageRate <= 0f ? KAnim.PlayMode.Once : KAnim.PlayMode.Loop);
                        break;
                    }
                }
            }
            else controller.Play("off");
        }

        [Serializable]
        public struct AnimRangeInfo
        {
            public float minFlow;
            public string animName;

            public AnimRangeInfo(float min_flow, string anim_name)
            {
                minFlow = min_flow;
                animName = anim_name;
            }
        }

    }
}
