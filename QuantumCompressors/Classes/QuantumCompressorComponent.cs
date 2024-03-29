﻿using KSerialization;
using QuantumCompressors.BuildingConfigs.Gas;
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
    [AddComponentMenu("KMonoBehaviour/scripts/" + nameof(QuantumCompressorComponent))]
    public class QuantumCompressorComponent : KMonoBehaviour, ISaveLoadable
    {
        public ConduitType conduitType;
        [MyCmpReq]
        private Operational _operational;
        private static readonly EventSystem.IntraObjectHandler<QuantumCompressorComponent> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<QuantumCompressorComponent>(((component, mustOperare) => component.OnOperationalChanged((bool)mustOperare)));
        private QuantumStorageManager _quantumStorageManager = QuantumStorageManager.Instance;
        [MyCmpReq] // Filter
        private Filterable _filterable;
        [MyCmpReq]
        private KSelectable _selectable;
        private void OnFilterChanged(Tag tag) => _selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, !tag.IsValid || tag == GameTags.Void);
        private static StatusItem _filterStatusItem;
        [MyCmpReq]
        private Storage _storage;
        private int _worldId;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            InitializeStatusItems();
            Subscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
        }

        public int GetWorldId()
        {
            return _worldId;
        }

        public float RemainingCapacity()
        {
            return _storage.RemainingCapacity();
        }

        public void AddElementChunk(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, bool do_disease_transfer = true)
        {
            switch (conduitType)
            {
                case ConduitType.Gas:
                    _storage.AddGasChunk(element, mass, temperature, disease_idx, disease_count, keep_zero_mass, do_disease_transfer);
                    break;
                case ConduitType.Liquid:
                    _storage.AddLiquid(element, mass, temperature, disease_idx, disease_count, keep_zero_mass, do_disease_transfer);
                    break;
            }
        }

        public bool IsOperational()
        {
            return _operational.IsOperational;
        }

        public Storage GetStorage()
        {
            return _storage;
        }

        protected override void OnSpawn()
        {
            OnOperationalChanged(_operational.IsOperational);
            base.OnSpawn();
            OnFilterChanged(_filterable.SelectedTag);
            _filterable.onFilterChanged += new Action<Tag>(OnFilterChanged);
            _selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, _filterStatusItem, this);
            _worldId = this.GetMyWorldId();
            _quantumStorageManager.AddStorage(this, conduitType);
            OnCmpEnable();
        }

        public Tag GetSelectedTag
        {
            get { return _filterable.SelectedTag; }
        }

        protected override void OnCleanUp()
        {
            Unsubscribe((int)GameHashes.OperationalChanged, OnOperationalChangedDelegate);
            _quantumStorageManager.RemoveStorage(this, conduitType);
            base.OnCleanUp();
        }

        private void OnOperationalChanged(bool mustOperate)
        {
            _operational.SetActive(mustOperate);
        }

        private void InitializeStatusItems()
        {
            if (_filterStatusItem != null)
                return;
            _filterStatusItem = new StatusItem("Filter", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.LiquidConduits.ID);
            _filterStatusItem.resolveStringCallback = ((str, data) =>
            {
                QuantumCompressorComponent elementFilter = (QuantumCompressorComponent)data;
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
            switch (((QuantumCompressorComponent)data).conduitType)
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

    }

}
