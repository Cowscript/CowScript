using script.builder;
using script.stack;

namespace script.variabel
{
    class StaticMethodVariabel : MethodVariabel
    {
        private ClassStaticMethods method;
        private ClassVariabel c;

        public StaticMethodVariabel(ClassStaticMethods m, ClassVariabel c) : base(null, null)
        {
            method = m;
            this.c = c;
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
            return db.createShadow(c);
        }

        public override CVar call(CVar[] call, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return method.call(c, db, call, data, pos);
        }
    }
}
