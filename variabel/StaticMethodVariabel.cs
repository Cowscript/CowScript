using script.builder;
using script.help;
using script.stack;

namespace script.variabel
{
    class StaticMethodVariabel : MethodVariabel
    {
        private ClassStaticMethods method;
        private ClassVariabel c;
        private VariabelDatabase extra;

        public StaticMethodVariabel(ClassStaticMethods m, ClassVariabel c, VariabelDatabase extra) : base(null, null, extra)
        {
            method = m;
            this.c = c;
            this.extra = extra;
        }

        public override int agumentSize()
        {
            return method.Aguments.size();
        }

        public override AgumentStack getStatck()
        {
            return method.Aguments;
        }

        public override VariabelDatabase getShadowVariabelDatabase(VariabelDatabase db)
        {
            if (extra != null)
                return extra.createShadow(c);

            return db.createShadow(c);
        }

        public override CVar call(CVar[] call, VariabelDatabase db, EnegyData data, Posision pos)
        {
            if(method.ReturnType != null)
            {
                CVar cache = method.call(c, db, call, data, pos);

                if(!CallScriptFunction.compare(cache, method.ReturnType))
                {
                    data.setError(new ScriptError("a static method '" + c.Name + "->" + method.Name + "' returns can not be convertet to '" + method.ReturnType + "'", pos), db);
                    return new NullVariabel();
                }

                return cache;
            }

            return method.call(c, db, call, data, pos);
        }
    }
}
