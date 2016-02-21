using script.builder;
using script.variabel;
using System.IO;

namespace script.plugin
{
    class Files : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            if(data.Config.get("file.system.enable", "false") != "true")
            {
                data.setError(new ScriptError("You can use the plugin 'file' becuse it not enabled in the config system!", new Posision(0, 0)), database);
                return;
            }

            Class f = new Class("File");

            ClassStaticMethods exsist = new ClassStaticMethods(f, "exists");
            exsist.Aguments.push("string", "dir");
            exsist.caller += Exsist_caller;
            exsist.create();

            ClassStaticMethods create = new ClassStaticMethods(f, "create");
            create.Aguments.push("string", "dir");
            create.Aguments.push("string", "contex", new NullVariabel());
            create.caller += Create_caller;
            create.create();

            ClassStaticMethods delete = new ClassStaticMethods(f, "delete");
            delete.Aguments.push("string", "dir");
            delete.caller += Delete_caller;
            delete.create();

            ClassStaticMethods write = new ClassStaticMethods(f, "write");
            write.Aguments.push("string", "dir");
            write.Aguments.push("string", "context");
            write.caller += Write_caller;
            write.create();

            ClassStaticMethods read = new ClassStaticMethods(f, "read");
            read.Aguments.push("string", "dir");
            read.caller += Read_caller;
            read.create();

            database.pushClass(f, data);
        }

        private CVar Read_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)c.getItem("exists").Context).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("'" + stack[0].toString(pos, data, db) + "' dont exists and there for can not be readed from", pos), db);
                return null;
            }

            try {
                return StringVariabel.CreateString(data, db, pos, System.IO.File.ReadAllText(stack[0].toString(pos, data, db)));
            }catch(IOException e)
            {
                data.setError(new ScriptError("Failed to read from '" + stack[0].toString(pos, data, db) + "': " + e.Message, pos), db);
            }

            return null;
        }

        private CVar Write_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)c.getItem("exists").Context).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("'" + stack[0].toString(pos, data, db) + "' dont exists and there for can not be writet in", pos), db);
                return null;
            }

            try {
                System.IO.File.WriteAllText(stack[0].toString(pos, data, db), stack[1].toString(pos, data, db));
            }catch(IOException e)
            {
                data.setError(new ScriptError("Failed to write to the file '" + stack[0].toString(pos, data, db) + "': " + e.Message, pos), db);
            }

            return null;
        }

        private CVar Delete_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)c.getItem("exists").Context).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("'" + stack[0].toString(pos, data, db) + "' dont exists and there for can not be deleted", pos), db);
                return null;
            }

            try {
                System.IO.File.Delete(stack[0].toString(pos, data, db));
            }catch(IOException e)
            {
                data.setError(new ScriptError("Failed to delete the file '" + stack[0].toString(pos, data, db) + "'. reasons: " + e.Message, pos), db);
            }

            return null;
        }

        private CVar Create_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(((StaticMethodVariabel)c.getItem("exists").Context).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("'" + stack[0].toString(pos, data, db) + "' exists and there for can not be createt", pos), db);
                return null;
            }

            FileStream fs;
            StreamWriter f = new StreamWriter((fs = System.IO.File.Create(stack[0].toString(pos, data, db))));

            if(!(stack[1] is NullVariabel))
            {
                try {
                    f.Write(stack[1].toString(pos, data, db));
                }catch(IOException e)
                {
                    fs.Close();
                    f.Close();
                    data.setError(new ScriptError("Write to the new file '" + stack[0].toString(pos, data, db) + "' failed: " + e.Message, pos), db);
                    return null;
                }
            }

            fs.Close();
            f.Close();

            return null;
        }

        private CVar Exsist_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(System.IO.File.Exists(stack[0].toString(pos, data, db)));
        }
    }
}
