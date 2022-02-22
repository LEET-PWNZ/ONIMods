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
        public readonly QuantumCompressorComponentList ActiveStorages;
        private QuantumStorageManager()
        {
            ActiveStorages = new QuantumCompressorComponentList();
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

    public class QuantumCompressorComponentList : List<QuantumCompressorComponent>
    {
        public QuantumCompressorComponentList():base(new List<QuantumCompressorComponent>()){}

        public new bool Contains(QuantumCompressorComponent item)
        {
            RemoveAll(i => i == null);
            return base.Contains(item);
        }

        public new void Add(QuantumCompressorComponent item)
        {
            RemoveAll(i => i == null);
            base.Add(item);
        }

        public new void Remove(QuantumCompressorComponent item)
        {
            RemoveAll(i => i == null);
            base.Remove(item);
        }


        public IEnumerable<QuantumCompressorComponent> FindStorage(Func<QuantumCompressorComponent, bool> predicate)
        {
            RemoveAll(i => i == null);
            return this.Where(predicate);
        }
    }

}
