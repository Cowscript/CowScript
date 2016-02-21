using System.Collections.Generic;

namespace script.plugin
{
    public class PluginContainer
    {
        private Dictionary<string, PluginInterface> container = new Dictionary<string, PluginInterface>();

        public PluginContainer()
        {
            //wee push plugin in this container :)
            container.Add("error",  new Error());
            container.Add("type",   new Types());
            container.Add("array",  new Array());
            container.Add("regex",  new Regex());
            container.Add("base64", new Base64());
            container.Add("time",   new Time());
            container.Add("random", new Random());
            container.Add("parser", new Parser());
            container.Add("math",   new Math());
            container.Add("config", new Config());
            container.Add("tcp",    new Tcp());
        }

        public bool exists(string name)
        {
            return container.ContainsKey(name);
        }

        public void open(VariabelDatabase database, string name, EnegyData data)
        {
            container[name].open(database, data);
        }
    }
}
