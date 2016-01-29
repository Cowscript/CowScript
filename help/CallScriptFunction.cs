using script.variabel;
using script.token;
using script.plugin;
using System.Collections;

namespace script.help
{
    class CallScriptFunction
    {
        private ArrayList body;
        private PluginContainer plugin;

        public CallScriptFunction(ArrayList body, PluginContainer plugin)
        {
            this.body = body;
            this.plugin = plugin;
        }

        public CVar call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            Interprenter.parse(new TokenCache(body, data, db), data, db);

            if(data.State == RunningState.Return)
            {
                //the state is return 
                return data.getReturn();
            }

            return new NullVariabel();
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
