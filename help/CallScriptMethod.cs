using script.stack;
using script.variabel;
using script.token;
using System.Collections;

namespace script.help
{
    class CallScriptMethod
    {
        public ArrayList Body { private set; get; }
        public AgumentStack Agument { set; get; }

        public CallScriptMethod(AgumentStack agument, ArrayList body)
        {
            Body = body;
            Agument = agument;
        }

        public CVar call(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            Interprenter interprenter = Interprenter.parse(db, data.Plugin, data);
            interprenter.setObject(obj);
            interprenter.parse(new TokenCache(Body));

            return interprenter.data.Return;
        }

        public CVar call(ClassVariabel c, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            Interprenter interprenter = Interprenter.parse(db, data.Plugin, data);
            interprenter.setStatic(c);
            interprenter.parse(new TokenCache(Body));

            return interprenter.data.Return;
        }
    }
}
