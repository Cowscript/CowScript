using script.builder;
using script.help;
using System;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Types : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Function type = new Function();
            type.Name = "type";
            type.agument.push("controls");
            type.agument.push("string", "isType");
            type.call += Type_call;
            database.pushFunction(type, data);

            Function toInt = new Function();
            toInt.Name = "toInt";
            toInt.agument.push("string", "context");
            toInt.call += ToInt_call;
            database.pushFunction(toInt, data);
        }

        private CVar Type_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            if (stack[0] is ObjectVariabel && stack[1].toString(pos, data, db) == ((ObjectVariabel)stack[0]).Name)
                return new BooleanVariabel(true);

            return new BooleanVariabel(stack[0].type() == stack[1].toString(pos, data, db));
        }

        private CVar ToInt_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            double result;
            if (!double.TryParse(stack[0].toString(pos, data, db), out result))
            {
                data.setError(new ScriptError(stack[0].toString(pos, data, db) + " could not be convertet to int", new Posision(0, 0)), db);
                return new NullVariabel();
            }

            return new IntVariabel(result);
        }
    }
}
