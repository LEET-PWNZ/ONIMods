using Newtonsoft.Json;
using System;
using System.IO;

namespace ONIModsLibrary.Classes
{
    public class ONIModConfigManager<T> where T: ONIModConfigBase<T>
    {
        public readonly float gasStorageCapacityMultiplier, liquidStorageCapacityMultiplier;

        private static ONIModConfigManager<T> _singleInstance;
        private readonly string _configDirPath, _configFileName;
        public readonly T CurrentConfig;
        private ONIModConfigManager()
        {
            _configDirPath = Directory.GetParent(System.Reflection.Assembly.GetAssembly(typeof(T)).Location).FullName + Path.DirectorySeparatorChar + "Config";
            _configFileName = _configDirPath+Path.DirectorySeparatorChar+ "conf"+typeof(T).Namespace +".json";
            CurrentConfig=SetupConfig();
        }

        private T SetupConfig()
        {
            T result = null;
            if (!Directory.Exists(_configDirPath)) Directory.CreateDirectory(_configDirPath);
            if (!File.Exists(_configFileName))
            {
                try
                {
                    T defaultConf = ((T)Activator.CreateInstance(typeof(T))).GetDefaultConfig();
                    string conf = JsonConvert.SerializeObject(defaultConf);
                    File.WriteAllText(_configFileName, conf);
                    result = defaultConf;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            else
            {
                try
                {
                    string fileContents = File.ReadAllText(_configFileName);
                    result = JsonConvert.DeserializeObject<T>(fileContents);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            
            return result;
        }

        public static ONIModConfigManager<T> Instance
        {
            get {
                if (_singleInstance == null) _singleInstance = new ONIModConfigManager<T>();
                return _singleInstance;
            }
        }
    }

    public abstract class ONIModConfigBase<T>
    {
        public abstract T GetDefaultConfig();
    }

}
