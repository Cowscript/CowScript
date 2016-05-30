using System.Collections.Generic;

namespace script.plugin
{
    public class PluginContainer
    {
        private Dictionary<string, PluginInterface> container = new Dictionary<string, PluginInterface>();

        public PluginContainer()
        {
            //wee push plugin in this container :)
            push(new Error());
            push(new TypePlugin());
            push(new Array());
            push(new Regex());
            push(new Base64());
            push(new Time());
            push(new Random());
            push(new Parser());
            push(new Math());
            push(new Config());
            push(new Tcp());
        }

        public void push(PluginInterface plugin)
        {
            container.Add(plugin.Name, plugin);
        }

        public bool exists(string name)
        {
            return container.ContainsKey(name);
        }

        public void open(VariabelDatabase database, string name, EnegyData data, Posision pos)
        {
            container[name].open(database, data, pos);
        }
    }
}
