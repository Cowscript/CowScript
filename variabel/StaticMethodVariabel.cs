using script.Container;
using script.stack;
using script.Type;

namespace script.variabel
{
    class StaticMethodVariabel : MethodVariabel
    {
        private MethodContainer method;
        private ClassVariabel c;
        private VariabelDatabase extra;

        public override bool SetVariabel
        {
            get
            {
                return method.SetVariabel;
            }
        }

        public StaticMethodVariabel(MethodContainer m, ClassVariabel c, VariabelDatabase extra) : base(null, null, extra)
        {
            method = m;
            this.c = c;
            this.extra = extra;
        }

        public override int agumentSize()
        {
            return method.Agument.size();
        }

        public override AgumentStack getStatck()
        {
            return method.Agument;
        }

        public override VariabelDatabase getShadowVariabelDatabase(VariabelDatabase db)
        {
            if (extra != null)
                return extra.createShadow(c);

            return db.createShadow(c);
        }

        public override CVar call(CVar[] call, VariabelDatabase db, EnegyData data, Posision pos)
        {
            CVar cache = method.Body(c, db, call, data, pos);
            db.garbageCollector();

            if (method.ReturnType != null)
            {

                if(!TypeHandler.controlType(cache, method.ReturnType))
                {
                    data.setError(new ScriptError("a static method '" + c.Name + "->" + method.Name + "' returns can not be convertet to '" + method.ReturnType + "'", pos), db);
                    return new NullVariabel();
                }
            }

            return cache;
        }
    }
}
