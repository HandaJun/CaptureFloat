using System;
using System.IO;
using YamlDotNet.Serialization;

namespace CaptureFloat
{
    [Serializable]
    public class ApplicationSetting
    {
        public static readonly string Location = "CaptureFloatSetting.yaml";

        public bool IsOnlyClipboard { get; set; } = false;
        public string ImgFolder { get; set; } = "";

        public static ApplicationSetting Load()
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(Location));
            if (!File.Exists(Location))
                using (var fs = File.Create(Location)) { }

            ApplicationSetting setting = null;
            try
            {
                setting = Deserialize(Location);
            }
            catch (Exception)
            {
            }

            if (setting == null)
            {
                setting = new ApplicationSetting();
                setting.Save();
            }

            return setting;
        }

        public void Save()
        {
            Serialize(this, Location);
        }

        public bool Ensure(bool isChanged = false)
        {
            //if (Level == 0)
            //{
            //    Level = 1;
            //    isChanged |= true;
            //}

            return isChanged;
        }

        private static void Serialize(ApplicationSetting setting, string path)
        {
            var serializer = new SerializerBuilder().Build();
            var yml = serializer.Serialize(setting);
            using (var sr = new StreamWriter(path))
            {
                sr.Write(yml);
            }
        }

        private static ApplicationSetting Deserialize(string path)
        {
            using (var sr = new StreamReader(path))
            {
                using (var input = new StringReader(sr.ReadToEnd()))
                {
                    var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                    return deserializer.Deserialize<ApplicationSetting>(input);
                }
            }
        }
    }
}


/***
 [App.xaml]
 public static ApplicationSetting Setting { get; set; }

 Setting = ApplicationSetting.Load();
 Setting.Save();
 ***/