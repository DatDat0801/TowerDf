using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Zitga.Localization
{
    public static class LocalizeMenu
    {
        private const string MENU_NAME = "Tools/Localization/Gen C#";
        // private const string EXTENSION = ".csv";

        [MenuItem(MENU_NAME, false, 0)]
        static void Generate()
        {
            CodeGenerator generator = new CodeGenerator();
            generator.GenFullProcess();

            AssetDatabase.Refresh();
        }
    }
    
    public class CodeGenerator
    {
        public const string SearchKey = "Localization/en";
        private const string ScriptPath = "/EW2/Src/Scripts/";
        private const string DefaultClassName = "L";
        string template = "using Zitga.Localization;\r\n${namespaces}\r\npublic static class ${name}\r\n{\r\n${properties}}";
        string subTemplate = "\r\n    public static class ${name}\r\n    {\r\n${properties}    }\r\n";
        string format = "        public static string {0} => Localization.Current.Get(\"{1}\", \"{2}\");";

        public string Generate(Dictionary<string, LanguageData> dict)
        {
            StringBuilder propertiesBuf = new StringBuilder();
            StringBuilder namespacesBuf = new StringBuilder();

            foreach (var kv in dict)
            {
                propertiesBuf.Append(SetUpClass(kv.Key, kv.Value));
            }

            var code = template.Replace("${namespaces}", namespacesBuf.ToString());
            code = code.Replace("${name}", DefaultClassName);
            code = code.Replace("${properties}", propertiesBuf.ToString());
            return code;
        }

        private string SetUpClass(string className, LanguageData data)
        {
            StringBuilder propertiesBuf = new StringBuilder();
            foreach (var key in data.data.Keys)
            {
                propertiesBuf.AppendFormat(format, GetPropertyName(key), className, key).Append(Environment.NewLine)
                    .Append(Environment.NewLine);
            }

            var code = subTemplate.Replace("${name}", className);
            code = code.Replace("${properties}", propertiesBuf.ToString());
            return code;
        }

        private string GetPropertyName(string key)
        {
            return Regex.Replace(key, "[.]", "_");
        }

        private Dictionary<string, LanguageData> CollectKeys()
        {
            var allFiles = Resources.LoadAll<LanguageData>(SearchKey);

            var keyDict = new Dictionary<string, LanguageData>();

            foreach (var file in allFiles)
            {
                keyDict.Add(file.name, file);
            }

            return keyDict;
        }

        public void WriteFile(string code)
        {
            // path to write code
            var writeFolder = Application.dataPath + ScriptPath;
            if (!Directory.Exists(writeFolder))
                Directory.CreateDirectory(writeFolder);

            File.WriteAllText(writeFolder + DefaultClassName + ".cs", code);
        }

        public void GenFullProcess()
        {
            var keyDict = CollectKeys();

            var code = Generate(keyDict);

            WriteFile(code);
        }
    }
}