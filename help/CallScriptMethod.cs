﻿using script.stack;
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

        public CVar call(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (obj is ObjectVariabel)
                return call((ObjectVariabel)obj, db, stack, data, pos);

            return call((ClassVariabel)obj, db, stack, data, pos);
        }

        public CVar call(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            //interprenter.setObject(obj);
            Interprenter.parse(new TokenCache(Body, data, db), data, db);

            return data.getReturn();
        }

        public CVar call(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            Interprenter.parse(new TokenCache(Body, data, db), data, db);

            return data.getReturn();
        }
    }
}
