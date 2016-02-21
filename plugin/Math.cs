using script.builder;
using script.variabel;

namespace script.plugin
{
    class Math : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Function cos = new Function();
            cos.Name = "cos";
            cos.agument.push("int", "");
            cos.call += Cos_call;

            database.pushFunction(cos, data);

            Function tan = new Function();
            tan.Name = "tan";
            tan.agument.push("int", "");
            tan.call += Tan_call;
            database.pushFunction(tan, data);

            Function sin = new Function();
            sin.Name = "sin";
            sin.agument.push("int", "");
            sin.call += Sin_call;
            database.pushFunction(sin, data);
        }

        private CVar Sin_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, System.Math.Sin(stack[0].toInt(pos, data, db)));
        }

        private CVar Tan_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, System.Math.Tan(stack[0].toInt(pos, data, db)));
        }

        private CVar Cos_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, System.Math.Cos(stack[0].toInt(pos, data, db)));
        }
    }
}
