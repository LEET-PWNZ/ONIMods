using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumCompressors.Classes.QuantumInputValve
{

    [SerializationConfig(MemberSerialization.OptIn)]
    public class QuantumOperationalInputValve:QuantumInputValveBase
    {
        [MyCmpReq]
        private Operational operational;
        private bool isDispensing;
        private static readonly EventSystem.IntraObjectHandler<QuantumOperationalInputValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<QuantumOperationalInputValve>((component, data) => component.OnOperationalChanged(data));

        private void OnOperationalChanged(object data)
        {
            bool dataVal = (bool)data;
            if (dataVal) CurrentFlow = MaxFlow;
            else CurrentFlow = 0.0f;
            operational.SetActive(dataVal);
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
        }

        protected override void OnSpawn()
        {
            OnOperationalChanged(operational.IsOperational);
            base.OnSpawn();
        }

        protected override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
            base.OnCleanUp();
        }

        protected override void OnMassTransfer(float amount) => isDispensing = amount > 0f;

        public override void UpdateAnim()
        {
            if (operational.IsOperational)
            {
                if (isDispensing) controller.Queue("on_flow", KAnim.PlayMode.Loop);
                else controller.Queue("on");
            }else controller.Queue("off");
        }


    }
}
