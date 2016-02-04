using script.builder;
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
            return method.call(c, db, call, data, pos);
        }
    }
}
