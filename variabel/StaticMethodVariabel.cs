using script.builder;
using script.stack;
using script.token;

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

        public override AgumentStack getStatck()
        {
            return method.Aguments;
        }

        public override CVar call(CallAgumentStack call, EnegyData data)
        {
            return method.call(c, data.VariabelDatabase, call, data);
        }
    }
}
