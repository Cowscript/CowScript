using System.Collections.Generic;

namespace script.help
{
    public class ScriptConfig
    {
        private Dictionary<string, ScriptConfigData> config = new Dictionary<string, ScriptConfigData>();

        public void set(string name, string context, bool canChange)
        {
            config.Add(name, new ScriptConfigData()
            {
                context = context,
                canChange = canChange
            });
        }

        public string get(string name, string defualts)
        {
            if (config.ContainsKey(name))
                return config[name].context;

            return defualts;
        }

        public bool canChange(string name)
        {
            if (config.ContainsKey(name))
                return config[name].canChange;

            return false;
        }

        public bool exists(string name)
        {
            return config.ContainsKey(name);
        }
    }

    class ScriptConfigData
    {
        public string context { set; get; }
        public bool canChange { set; get; }
    }
}
