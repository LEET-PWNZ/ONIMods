using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumCompressors.Classes
{
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/scripts/" + nameof(QuantumOperationalOutlet))]
    public class QuantumOperationalOutlet : KMonoBehaviour, ISaveLoadable,ISecondaryOutput
    {
        [MyCmpReq] // Operational
        private Operational _operational;
        private static readonly EventSystem.IntraObjectHandler<QuantumOperationalOutlet> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<QuantumOperationalOutlet>(((component, mustOperare) => component.OnOperationalChanged((bool)mustOperare)));
        private bool _isDispensing;
        public ConduitPortInfo portInfo;
        [MyCmpGet]
        protected KBatchedAnimController controller;
        protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;

        [MyCmpReq] // Filter
        private Filterable _filterable;
        private int _filteredCell = -1;
        private static StatusItem _filterStatusItem;
        private FlowUtilityNetwork.NetworkItem _itemFilter;
        [MyCmpReq]
        private KSelectable _selectable;
        private Guid _needsConduitStatusItemGuid;
        private Guid _conduitBlockedStatusItemGuid;
        private HandleVector<int>.Handle _partitionerEntry;

        [MyCmpReq]
        public Building building;
        [SerializeField]
        private List<QuantumCompressorComponent> _compressors = new List<QuantumCompressorComponent>();
        [SerializeField]
        private List<Storage> _compressorStorages = new List<Storage>();
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;
        private int _worldId;
        private void OnOperationalChanged(bool mustOperate)=> _operational.SetActive(mustOperate);

        protected void OnMassTransfer(float amount) => _isDispensing = amount > 0f;
        private void OnFilterChanged(Tag tag)=> _selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, !tag.IsValid || tag == GameTags.Void);
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            InitializeStatusItems();
            flowAccumulator = Game.Instance.accumulators.Add("Flow", this);
            Subscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
        }

        protected override void OnSpawn()
        {
            OnOperationalChanged(_operational.IsOperational);
            base.OnSpawn();
            _worldId = this.GetMyWorldId();
            _filteredCell = Grid.OffsetCell(Grid.PosToCell(transform.GetPosition()), building.GetRotatedOffset(portInfo.offset));
            IUtilityNetworkMgr utilityNetworkMgr = Conduit.GetNetworkManager(portInfo.conduitType);
            _itemFilter = new FlowUtilityNetwork.NetworkItem(portInfo.conduitType, Endpoint.Source, _filteredCell, gameObject);
            utilityNetworkMgr.AddToNetworks(_filteredCell, _itemFilter, true);
            OnFilterChanged(_filterable.SelectedTag);
            _filterable.onFilterChanged += new Action<Tag>(OnFilterChanged);
            Conduit.GetFlowManager(portInfo.conduitType).AddConduitUpdater(new Action<float>(ConduitUpdate), ConduitFlowPriority.Default);
            _selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, _filterStatusItem, this);
            UpdateConduitExistsStatus();
            UpdateConduitBlockedStatus();
            ScenePartitionerLayer layer = null;
            switch (portInfo.conduitType)
            {
                case ConduitType.Gas:
                    layer = GameScenePartitioner.Instance.gasConduitsLayer;
                    break;
                case ConduitType.Liquid:
                    layer = GameScenePartitioner.Instance.liquidConduitsLayer;
                    break;
            }
            _partitionerEntry = GameScenePartitioner.Instance.Add("ElementFilterConduitExists", gameObject, _filteredCell, layer, data => UpdateConduitExistsStatus());
            UpdateAnim();
            OnCmpEnable();
        }

        private void UpdateCompressors()
        {
            _compressors = _quantumStorageManager
                .FindStorage(portInfo.conduitType, _worldId);
            _compressorStorages = _compressors.Select(c => c.GetStorage()).ToList();
        }

        private void ConduitUpdate(float dt)
        {
            UpdateCompressors();
            UpdateConduitBlockedStatus();
            bool activeFlag = false;
            ConduitFlow flowManager = Conduit.GetFlowManager(portInfo.conduitType);
            if (!flowManager.HasConduit(_filteredCell) || !_compressors.Any() || !_operational.IsOperational)
            {
                OnMassTransfer(0.0f);
                UpdateAnim();
            }
            else
            {
                Storage usedStorage = null;
                PrimaryElement transferElement = FindSuitableElement(out usedStorage);
                float transferMass = 0f;
                if (transferElement != null)
                {
                    ConduitFlow.ConduitContents filterCellContents = flowManager.GetContents(_filteredCell);
                    if (filterCellContents.mass <= 0f)
                    {
                        transferMass = flowManager.AddElement(_filteredCell, transferElement.ElementID, transferElement.Mass, transferElement.Temperature, transferElement.DiseaseIdx, transferElement.DiseaseCount);
                        if (transferMass > 0f)
                        {
                            activeFlag = true;
                            int disease_count = (int)(transferElement.DiseaseCount * (transferMass / transferElement.Mass));
                            transferElement.ModifyDiseaseCount(-disease_count, "QuantumOperationalOutlet.ConduitUpdate");
                            transferElement.Mass -= transferMass;
                            Game.Instance.accumulators.Accumulate(flowAccumulator, transferMass);
                            usedStorage?.Trigger((int)GameHashes.OnStorageChange, transferElement.gameObject);
                        }
                    }
                }
                OnMassTransfer(transferMass);
                UpdateAnim();
            }
            _operational.SetActive(activeFlag);
        }
        private int _elementOutputOffset = 0;
        private PrimaryElement FindSuitableElement(out Storage usedStorage)
        {
            // Iterate in reverse for LIFO buffering
            for (int i = _compressorStorages.Count(); i > 0; i--)
            {
                List<GameObject> availableItems = _compressorStorages[i].items.Where(s => s.GetComponent<PrimaryElement>().ElementID.CreateTag() == _filterable.SelectedTag).ToList();
                int filteredCount = availableItems.Count;
                for (int j = 0; j < filteredCount; j++)
                {
                    int elemIndex = (j + _elementOutputOffset) % filteredCount;
                    PrimaryElement component = availableItems[elemIndex].GetComponent<PrimaryElement>();
                    if (component != null && component.Mass > 0f)
                    {
                        _elementOutputOffset = (_elementOutputOffset + 1) % filteredCount;
                        usedStorage = _compressorStorages[i];
                        return component;
                    }
                }
            }
            usedStorage = null;
            return null;
        }

        protected override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
            Game.Instance.accumulators.Remove(flowAccumulator);
            Conduit.GetFlowManager(portInfo.conduitType).RemoveConduitUpdater(new Action<float>(ConduitUpdate));
            Conduit.GetNetworkManager(portInfo.conduitType).RemoveFromNetworks(_filteredCell, _itemFilter, true);
            if (_partitionerEntry.IsValid() && GameScenePartitioner.Instance != null) GameScenePartitioner.Instance.Free(ref _partitionerEntry);
            base.OnCleanUp();
        }

        private void UpdateConduitExistsStatus()
        {
            bool outputConnected = RequireOutputs.IsConnected(_filteredCell, portInfo.conduitType);
            StatusItem status_item = portInfo.conduitType == ConduitType.Gas ? Db.Get().BuildingStatusItems.NeedGasOut : Db.Get().BuildingStatusItems.NeedLiquidOut;
            bool needItemNotEmpty = _needsConduitStatusItemGuid != Guid.Empty;
            if (outputConnected != needItemNotEmpty)
                return;
            _needsConduitStatusItemGuid = _selectable.ToggleStatusItem(status_item, _needsConduitStatusItemGuid, !outputConnected);
        }

        private void UpdateConduitBlockedStatus()
        {
            bool conduitEmpty = Conduit.GetFlowManager(portInfo.conduitType).IsConduitEmpty(_filteredCell);
            StatusItem blockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
            bool blockedItemEmpty = _conduitBlockedStatusItemGuid != Guid.Empty;
            if (conduitEmpty != blockedItemEmpty)
                return;
            _conduitBlockedStatusItemGuid = _selectable.ToggleStatusItem(blockedMultiples, _conduitBlockedStatusItemGuid, !conduitEmpty);
        }

        private void InitializeStatusItems()
        {
            if (_filterStatusItem != null)
                return;
            _filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID);
            _filterStatusItem.resolveStringCallback = ((str, data) =>
            {
                QuantumOperationalOutlet elementFilter = (QuantumOperationalOutlet)data;
                str = !elementFilter._filterable.SelectedTag.IsValid ||
                elementFilter._filterable.SelectedTag == GameTags.Void ?
                string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, BUILDINGS.PREFABS.GASFILTER.ELEMENT_NOT_SPECIFIED) :
                string.Format(BUILDINGS.PREFABS.GASFILTER.STATUS_ITEM, elementFilter._filterable.SelectedTag.ProperName());
                return str;
            });
            _filterStatusItem.conditionalOverlayCallback = new Func<HashedString, object, bool>(ShowInUtilityOverlay);
        }

        private bool ShowInUtilityOverlay(HashedString mode, object data)
        {
            bool result = false;
            switch (((QuantumOperationalOutlet)data).portInfo.conduitType)
            {
                case ConduitType.Gas:
                    result = mode == OverlayModes.GasConduits.ID;
                    break;
                case ConduitType.Liquid:
                    result = mode == OverlayModes.LiquidConduits.ID;
                    break;
            }
            return result;
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

        public bool HasSecondaryConduitType(ConduitType type) => portInfo.conduitType == type;
        public int GetFilteredCell() => _filteredCell;
        public CellOffset GetSecondaryConduitOffset(ConduitType type) => portInfo.offset;

    }
}
