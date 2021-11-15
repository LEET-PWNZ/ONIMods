using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ONIModsLibrary.Classes
{
    public class ONIModConfigManager<T> where T: ONIModConfigBase<T>
    {
        public readonly float gasStorageCapacityMultiplier, liquidStorageCapacityMultiplier;

        private static ONIModConfigManager<T> singleInstance;
        private readonly string configDirPath, configFileName;
        public readonly T CurrentConfig;
        private ONIModConfigManager()
        {
            configDirPath = Directory.GetParent(System.Reflection.Assembly.GetAssembly(typeof(T)).Location).FullName + Path.DirectorySeparatorChar + "Config";
            configFileName = configDirPath+Path.DirectorySeparatorChar+ "conf"+typeof(T).Namespace +".json";
            CurrentConfig=SetupConfig();
        }

        private T SetupConfig()
        {
            T result = null;
            if (!Directory.Exists(configDirPath)) Directory.CreateDirectory(configDirPath);
            if (!File.Exists(configFileName))
            {
                try
                {
                    T defaultConf = ((T)Activator.CreateInstance(typeof(T))).GetDefaultConfig();
                    string conf = JsonConvert.SerializeObject(defaultConf);
                    File.WriteAllText(configFileName, conf);
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
                    string fileContents = File.ReadAllText(configFileName);
                    result = JsonConvert.DeserializeObject<T>(fileContents);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            
            return result;
        }

        public static ONIModConfigManager<T> getInstance() 
        {
            if (singleInstance == null) singleInstance = new ONIModConfigManager<T>();
            return singleInstance;
        }
    }

    public abstract class ONIModConfigBase<T>
    {
        public abstract T GetDefaultConfig();
    }

}
