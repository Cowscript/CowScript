using script.builder;
using script.Type;
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

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            if(data.Config.get("tcp.enable", "false") != "true")
            {
                data.setError(new ScriptError("Tcp is not enable. to enable tcp set config 'tcp.enable' to 'true'", new Posision(0, 0)), database);
                return;
            }

            Class tcp = new Class("Tcp");

            Pointer ie = new Pointer("ie");
            ie.SetPrivate();
            tcp.SetPointer(ie, data, database, pos);

            Pointer em = new Pointer("em");
            em.SetPrivate();
            tcp.SetPointer(em, data, database, pos);

            Method constructor = new Method("");
            constructor.GetAgumentStack().push("string", "host");
            constructor.GetAgumentStack().push("int", "port");
            constructor.SetBody(Constructor_caller);
            tcp.SetConstructor(constructor, data, database, pos);

            Method writeLine = new Method("writeLine");
            writeLine.GetAgumentStack().push("string", "message");
            writeLine.SetBody(WriteLine_caller);
            tcp.SetMethod(writeLine, data, database, pos);

            Method readLine = new Method("readLine");
            readLine.SetReturnType("string");
            readLine.SetBody(ReadLine_caller);
            tcp.SetMethod(readLine, data, database, pos);

            Method isError = new Method("isError");
            isError.SetReturnType("bool");
            isError.SetBody(IsError_caller);
            tcp.SetMethod(isError, data, database, pos);

            Method getError = new Method("getError");
            getError.SetReturnType("string");
            getError.SetBody(GetError_caller);
            tcp.SetMethod(getError, data, database, pos);
            
            database.pushClass(tcp, data);
        }

        private CVar GetError_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return TypeHandler.ToObjectVariabel(obj).get("em");
        }

        private CVar IsError_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return TypeHandler.ToObjectVariabel(obj).get("ie");
        }

        private CVar ReadLine_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(TypeHandler.ToObjectVariabel(obj).get("ie").toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("Tou can not use 'Tcp->readLine()' when there are error in tcp connection", pos), db);
                return null;
            }

            try {
                string respons = ((StreamReader)TypeHandler.ToObjectVariabel(obj).systemItems["reader"]).ReadLine();

                if (respons == null)
                    return new NullVariabel();

                return Types.toString(respons, data, db, pos);
            }catch(IOException e)
            {
                setError(Types.toString(e.Message, data, db, pos), TypeHandler.ToObjectVariabel(obj));
                return new NullVariabel();
            }
        }

        private CVar WriteLine_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if(TypeHandler.ToObjectVariabel(obj).get("ie").toBoolean(pos, data, db))
            {
                data.setError(new ScriptError("You can not use 'Tcp->writeLine(string)' when there are error in tcp connection", pos), db);
                return null;
            }

            try {
                ((StreamWriter)TypeHandler.ToObjectVariabel(obj).systemItems["writer"]).WriteLine(stack[0].toString(pos, data, db));
                return new BooleanVariabel(true);
            }catch(IOException e)
            {
                setError(Types.toString(e.Message, data, db, pos), TypeHandler.ToObjectVariabel(obj));
                return new BooleanVariabel(false);
            }
            
        }

        private CVar Constructor_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            try {
                TcpClient client = new TcpClient(stack[0].toString(pos, data, db), Convert.ToInt32(stack[1].toInt(pos, data, db)));
                TypeHandler.ToObjectVariabel(obj).systemItems.Add("client", client);

                StreamWriter writer = new StreamWriter(client.GetStream());
                TypeHandler.ToObjectVariabel(obj).systemItems.Add("writer", writer);

                StreamReader reader = new StreamReader(client.GetStream());
                TypeHandler.ToObjectVariabel(obj).systemItems.Add("reader", reader);
            }catch(SocketException se)
            {
                setError(Types.toString(se.Message, data, db, pos), TypeHandler.ToObjectVariabel(obj));
            }

            return null;
        }
    }
}
