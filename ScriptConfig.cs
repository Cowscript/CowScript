using System.Collections.Generic;

namespace script
{
    public class ScriptConfig
    {
        private Dictionary<string, ScriptConfigData> container = new Dictionary<string, ScriptConfigData>();
        public bool isScriptLock {set; get; }

        public ScriptConfig()
        {
            isScriptLock = false;
        }

        public void append(string name, string context, bool allowOveride)
        {
            if (container.ContainsKey(name))
            {
                if (!container[name].AllowOveride)
                    allowOveride = false;

                container.Remove(name);
            }

            container.Add(name, new ScriptConfigData()
            {
                Context = context,
                AllowOveride = allowOveride
            });
        }

        public string get(string name, string defualtValue)
        {
            if (container.ContainsKey(name))
                return container[name].Context;

            return defualtValue;
        }

        public bool isAllowedOverride(string name)
        {
            if (container.ContainsKey(name))
                return container[name].AllowOveride;

            return true;
        }

        public bool exists(string name)
        {
            return container.ContainsKey(name);
        }
    }

    class ScriptConfigData
    {
        public string Context {get; set; }
        public bool AllowOveride { get; set; }

    }
}
