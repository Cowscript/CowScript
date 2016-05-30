using script.builder;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Math : PluginInterface
    {
        public string Name { get { return "math"; } }
        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Function cosh = new Function();
            cosh.Name = "cosh";
            cosh.agument.push("int", "");
            cosh.call += Cosh_call;

            database.pushFunction(cosh, data);

            Function cos = new Function();
            cos.Name = "cos";
            cos.agument.push("int", "");
            cos.call += Cos_call;

            database.pushFunction(cos, data);

            Function tanh = new Function();
            tanh.Name = "tanh";
            tanh.agument.push("int", "");
            tanh.call += Tanh_call;
            database.pushFunction(tanh, data);

            Function tan = new Function();
            tan.Name = "tan";
            tan.agument.push("int", "");
            tan.call += Tan_call;
            database.pushFunction(tan, data);

            Function sinh = new Function();
            sinh.Name = "sinh";
            sinh.agument.push("int", "");
            sinh.call += Sinh_call;
            database.pushFunction(sinh, data);

            Function sin = new Function();
            sin.Name = "sin";
            sin.agument.push("int", "");
            sin.call += Sin_call;
            database.pushFunction(sin, data);
        }

        private CVar Sinh_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Sinh(stack[0].toInt(pos, data, db)), data, db, pos);
        }

        private CVar Tanh_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Tanh(stack[0].toInt(pos, data, db)), data, db, pos);
        }

        private CVar Cosh_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Cosh(stack[0].toInt(pos, data, db)), data, db, pos);
        }

        private CVar Sin_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Sin(stack[0].toInt(pos, data, db)), data, db, pos);
        }

        private CVar Tan_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Tan(stack[0].toInt(pos, data, db)), data, db, pos);
        }

        private CVar Cos_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toInt(System.Math.Cos(stack[0].toInt(pos, data, db)), data, db, pos);
        }
    }
}
