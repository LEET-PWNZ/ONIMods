using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumCompressors.Classes
{
    public class QuantumStorageManager
    {
        private static QuantumStorageManager _instance;
        private readonly List<QuantumCompressorComponent> _activeGasStorages;
        private readonly List<QuantumCompressorComponent> _activeLiquidStorages;
        private QuantumStorageManager()
        {
            _activeGasStorages = new List<QuantumCompressorComponent>();
            _activeLiquidStorages = new List<QuantumCompressorComponent>();
        }

        public void AddStorage(QuantumCompressorComponent storage, ConduitType conduitType)
        {
            switch (conduitType)
            {
                case ConduitType.Gas:
                    if (!_activeGasStorages.Contains(storage)) _activeGasStorages.Add(storage);
                    break;
                case ConduitType.Liquid:
                    if (!_activeLiquidStorages.Contains(storage)) _activeLiquidStorages.Add(storage);
                    break;
            }
        }

        public void RemoveStorage(QuantumCompressorComponent storage, ConduitType conduitType)
        {
            switch (conduitType)
            {
                case ConduitType.Gas:
                    if (_activeGasStorages.Contains(storage)) _activeGasStorages.Remove(storage);
                    break;
                case ConduitType.Liquid:
                    if (_activeLiquidStorages.Contains(storage)) _activeLiquidStorages.Remove(storage);
                    break;
            }
        }

        public List<QuantumCompressorComponent> FindStorage(ConduitType conduitType, int worldId)
        {
            List<QuantumCompressorComponent> returnResult = new List<QuantumCompressorComponent>();
            switch (conduitType)
            {
                case ConduitType.Gas:
                    _activeGasStorages.RemoveAll(i => i == null);
                    returnResult = _activeGasStorages.Where(s => s.GetWorldId() == worldId && s.IsOperational()).ToList();
                    break;
                case ConduitType.Liquid:
                    _activeLiquidStorages.RemoveAll(i => i == null);
                    returnResult = _activeLiquidStorages.Where(s => s.GetWorldId() == worldId && s.IsOperational()).ToList();
                    break;
            }
            return returnResult;
        }

        public static QuantumStorageManager Instance
        {
            get
            {
                if (_instance == null) _instance = new QuantumStorageManager();
                return _instance;
            }
        }

    }

}
