using script.builder;
using script.Type;
using script.variabel;
using System.IO;

namespace script.plugin
{
    class Files : PluginInterface
    {

        public string Name { get { return "file"; } }

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            if(data.Config.get("file.system.enable", "false") != "true")
            {
                data.setError(new ScriptError("You can use the plugin 'file' becuse it not enabled in the config system!", new Posision(0, 0)), database);
                return;
            }

            Class f = new Class("File");

            Method exsist = new Method("exists");
            exsist.SetStatic();
            exsist.GetAgumentStack().push("string", "dir");
            exsist.SetBody(Exsist_caller);
            f.SetMethod(exsist, data, database, pos);

            Method create = new Method("create");
            create.SetStatic();
            create.GetAgumentStack().push("string", "dir");
            create.GetAgumentStack().push("string", "context", new NullVariabel());
            create.SetBody(Create_caller);
            f.SetMethod(create, data, database, pos);

            Method delete = new Method("delete");
            delete.SetStatic();
            delete.GetAgumentStack().push("string", "dir");
            delete.SetBody(Delete_caller);
            f.SetMethod(delete, data, database, pos);

            Method write = new Method("write");
            write.SetStatic();
            write.GetAgumentStack().push("string", "dir");
            write.GetAgumentStack().push("string", "context");
            write.SetBody(Write_caller);
            f.SetMethod(write, data, database, pos);

            Method read = new Method("read");
            read.SetStatic();
            read.GetAgumentStack().push("string", "dir");
            read.SetBody(Read_caller);
            f.SetMethod(read, data, database, pos);

            database.pushClass(f, data);
        }

        private CVar Read_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)TypeHandler.ToObjectVariabel(c).get("exists")).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("'" + stack[0].toString(pos, data, db) + "' dont exists and there for can not be readed from", pos), db);
                return null;
            }

            try {
                return Types.toString(System.IO.File.ReadAllText(stack[0].toString(pos, data, db)), data, db, pos);
            }catch(IOException e)
            {
                data.setError(new ScriptError("Failed to read from '" + stack[0].toString(pos, data, db) + "': " + e.Message, pos), db);
            }

            return null;
        }

        private CVar Write_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)TypeHandler.ToObjectVariabel(c).get("exists")).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
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

        private CVar Delete_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (!((StaticMethodVariabel)TypeHandler.ToObjectVariabel(c).get("exists")).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
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

        private CVar Create_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(((StaticMethodVariabel)TypeHandler.ToObjectVariabel(c).get("exists")).call(data, db, stack[0].toString(pos, data, db)).toBoolean(pos, data, db))
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

        private CVar Exsist_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(System.IO.File.Exists(stack[0].toString(pos, data, db)));
        }
    }
}
