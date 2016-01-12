using script.stack;
using script.variabel;
using script.token;
using script.plugin;
using System.Collections;

namespace script.help
{
    class CallScriptFunction : CallInterface
    {
        private ArrayList body;
        private PluginContainer plugin;

        public CallScriptFunction(ArrayList body, PluginContainer plugin)
        {
            this.body = body;
            this.plugin = plugin;
        }

        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            Interprenter interprenter = Interprenter.parse(data.VariabelDatabase, plugin, data);
            interprenter.parse(new TokenCache(body));

            if(interprenter.data.Return == null)
                return new NullVariabel();

            return interprenter.data.Return;
        }

        public static bool compare(CVar var, string type)
        {
            if (type == "function" && var.type() == "method")
                return true;//method can also be a function :)

            if (type == "string" && var.type() == "int")
                return true;//int can be convertet to string :)

            return var.type().Equals(type) || var is ObjectVariabel && type == ((ObjectVariabel)var).Name;
        }
    }
}
