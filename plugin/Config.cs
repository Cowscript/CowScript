using script.builder;
using script.variabel;

namespace script.plugin
{
    class Config : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Class config = new Class("Config", data, database);

            ClassStaticMethods get = new ClassStaticMethods(config, "get", true, "string");
            get.Aguments.push("string", "name");
            get.caller += Get_caller;
            get.create();

            ClassStaticMethods isScriptLock = new ClassStaticMethods(config, "isScriptLock", true, "bool");
            isScriptLock.caller += IsScriptLock_caller;
            isScriptLock.create();

            ClassStaticMethods set = new ClassStaticMethods(config, "set");
            set.Aguments.push("string", "name");
            set.Aguments.push("string", "value");
            set.caller += Set_caller;
            set.create();

            ClassStaticMethods isLocked = new ClassStaticMethods(config, "isLocked", true, "bool");
            isLocked.Aguments.push("string", "name");
            isLocked.caller += IsLocked_caller;
            isLocked.create();

            database.pushClass(config, data);
        }

        private CVar IsLocked_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            string name = stack[0].toString(pos, data, db);

            if (!data.Config.exists(name))
            {
                data.setError(new ScriptError("Unknown config: " + name, pos), db);
                return new BooleanVariabel(false);
            }

            return new BooleanVariabel(data.Config.isAllowedOverride(name));
        }

        private CVar Set_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (data.Config.isScriptLock)
            {
                data.setError(new ScriptError("The config is locked", pos), db);
                return null;
            }

            string name = stack[0].toString(pos, data, db);

            if (data.Config.exists(name) && data.Config.isAllowedOverride(name))
            {
                data.setError(new ScriptError("The config '" + name + "' is not allowed to be changed by the script", pos), db);
                return new NullVariabel();
            }

            data.Config.append(name, stack[1].toString(pos, data, db), true);
            return null;
        }

        private CVar IsScriptLock_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(data.Config.isScriptLock);
        }

        private CVar Get_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            string name = stack[0].toString(pos, data, db);

            if (!data.Config.exists(name))
            {
                data.setError(new ScriptError("Unknown config name: " + name, pos), db);
                return new NullVariabel();
            }

            return StringVariabel.CreateString(data, db, pos, data.Config.get(name, ""));
        }
    }
}
