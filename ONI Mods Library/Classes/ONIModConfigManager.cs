using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            T defaultConf = ((T)Activator.CreateInstance(typeof(T))).GetDefaultConfig();
            if (!Directory.Exists(_configDirPath)) Directory.CreateDirectory(_configDirPath);
            if (!File.Exists(_configFileName))
            {
                try
                {
                    SaveFile(defaultConf);
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
                    var readConfig = JsonConvert.DeserializeObject<T>(fileContents);
                    result = MergeCurrentWithDefault(readConfig, defaultConf);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            
            return result;
        }

        private T MergeCurrentWithDefault(T currentConfig,T defaultConfig)
        {
            T newConfig = currentConfig;
            bool mustSave = false;
            foreach (var prop in currentConfig.GetType().GetProperties())
            {
                var currentPropVal = prop.GetValue(currentConfig);
                var defaultPropVal= prop.GetValue(defaultConfig);
                if (currentPropVal == null || string.IsNullOrEmpty(currentPropVal.ToString()))
                {
                    prop.SetValue(newConfig, defaultPropVal);
                    if(!mustSave) mustSave = true;
                }
            }
            if (mustSave)
            {
                SaveFile(newConfig);
            }
            return newConfig;
        }

        private void SaveFile(T objToSave)
        {
            string conf = JsonConvert.SerializeObject(objToSave);
            File.WriteAllText(_configFileName, conf);
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
