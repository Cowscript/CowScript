using script.builder;
using script.variabel;
using System;
using System.IO;
using System.Net.Sockets;

namespace script.plugin
{
    class Tcp : PluginInterface
    {
        public static void setError(CVar msg, ObjectVariabel obj)
        {
            obj.appendToPointer("ie", new BooleanVariabel(true));
            obj.appendToPointer("em", msg);
        }

        public void open(VariabelDatabase database, EnegyData data)
        {
            if(data.Config.get("tcp.enable", "false") != "true")
            {
                data.setError(new ScriptError("Tcp is not enable. to enable tcp set config 'tcp.enable' to 'true'", new Posision(0, 0)), database);
                return;
            }

            Class tcp = new Class("Tcp");

            tcp.addVariabel("ie", new BooleanVariabel(false), false, false);
            tcp.addVariabel("em", new NullVariabel(), false, false);

            ClassMethods constructor = new ClassMethods(tcp, "");
            constructor.Aguments.push("string", "host");
            constructor.Aguments.push("int", "port");
            constructor.caller += Constructor_caller;
            constructor.createConstructor();

            ClassMethods writeLine = new ClassMethods(tcp, "writeLine");
            writeLine.caller += WriteLine_caller;
            writeLine.Aguments.push("string", "message");
            writeLine.create();

            ClassMethods readLine = new ClassMethods(tcp, "readLine", true, "string");
            readLine.caller += ReadLine_caller;
            readLine.create();

            ClassMethods isError = new ClassMethods(tcp, "isError", true, "bool");
            isError.caller += IsError_caller;
            isError.create();

            ClassMethods getError = new ClassMethods(tcp, "getError", true, "string");
            getError.caller += GetError_caller;
            getError.create();
            
            database.pushClass(tcp, data);
        }

        private CVar GetError_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj.get("em");
        }

        private CVar IsError_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj.get("ie");
        }

        private CVar ReadLine_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(obj.get("ie").toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("Tou can not use 'Tcp->readLine()' when there are error in tcp connection", pos), db);
                return null;
            }

            try {
                string respons = ((StreamReader)obj.systemItems["reader"]).ReadLine();

                if (respons == null)
                    return new NullVariabel();

                return StringVariabel.CreateString(data, db, pos, respons);
            }catch(IOException e)
            {
                setError(StringVariabel.CreateString(data, db, pos, e.Message), obj);
                return new NullVariabel();
            }
        }

        private CVar WriteLine_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(obj.get("ie").toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("You can not use 'Tcp->writeLine(string)' when there are error in tcp connection", pos), db);
                return null;
            }

            try {
                ((StreamWriter)obj.systemItems["writer"]).WriteLine(stack[0].toString(pos, data, db));
                return new BooleanVariabel(true);
            }catch(IOException e)
            {
                setError(StringVariabel.CreateString(data, db, pos, e.Message), obj);
                return new BooleanVariabel(false);
            }
            
        }

        private CVar Constructor_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            try {
                TcpClient client = new TcpClient(stack[0].toString(pos, data, db), Convert.ToInt32(stack[1].toInt(pos, data, db)));
                obj.systemItems.Add("client", client);

                StreamWriter writer = new StreamWriter(client.GetStream());
                obj.systemItems.Add("writer", writer);

                StreamReader reader = new StreamReader(client.GetStream());
                obj.systemItems.Add("reader", reader);
            }catch(SocketException se)
            {
                setError(StringVariabel.CreateString(data, db, pos, se.Message), obj);
            }

            return null;
        }
    }
}
