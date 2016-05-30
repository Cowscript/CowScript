using script.builder;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Config : PluginInterface
    {
        public string Name { get { return "config"; } }

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class config = new Class("Config");

            Method get = new Method("get");
            get.GetAgumentStack().push("string", "name");
            get.SetBody(Get_caller);
            get.SetStatic();
            config.SetMethod(get, data, database, pos);

            Method isScriptLock = new Method("isScriptLock");
            isScriptLock.SetBody(IsScriptLock_caller);
            isScriptLock.SetStatic();
            config.SetMethod(isScriptLock, data, database, pos);

            Method set = new Method("set");
            set.GetAgumentStack().push("string", "name");
            set.GetAgumentStack().push("string", "value");
            set.SetBody(Set_caller);
            set.SetStatic();
            config.SetMethod(set, data, database, pos);

            Method isLocked = new Method("isLocked");
            isLocked.SetStatic();
            isLocked.GetAgumentStack().push("string", "name");
            isLocked.SetBody(IsLocked_caller);
            config.SetMethod(isLocked, data, database, pos);

            database.pushClass(config, data);
        }

        private CVar IsLocked_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            string name = stack[0].toString(pos, data, db);

            if (!data.Config.exists(name))
            {
                data.setError(new ScriptError("Unknown config: " + name, pos), db);
                return new BooleanVariabel(false);
            }

            return new BooleanVariabel(data.Config.isAllowedOverride(name));
        }

        private CVar Set_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
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

        private CVar IsScriptLock_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(data.Config.isScriptLock);
        }

        private CVar Get_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            string name = stack[0].toString(pos, data, db);

            if (!data.Config.exists(name))
            {
                data.setError(new ScriptError("Unknown config name: " + name, pos), db);
                return new NullVariabel();
            }

            return Types.toString(data.Config.get(name, ""), data, db, pos);
        }
    }
}
