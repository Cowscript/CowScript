using System.Collections.Generic;

namespace script.plugin
{
    public class PluginContainer
    {
        private Dictionary<string, PluginInterface> container = new Dictionary<string, PluginInterface>();

        public PluginContainer()
        {
            //wee push plugin in this container :)
            container.Add("error", new Error());
            container.Add("type",  new Types());
            container.Add("array", new Array());
            container.Add("regex", new Regex());
            container.Add("base64", new Base64());
        }

        public bool exists(string name)
        {
            return container.ContainsKey(name);
        }

        public void open(VariabelDatabase database, string name)
        {
            container[name].open(database);
        }
    }
}
