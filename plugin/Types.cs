using script.builder;
using script.help;
using System;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Types : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Function type = new Function();
            type.Name = "type";
            type.agument.push("controls");
            type.agument.push("string", "isType");
            type.call = new TypeControler();
            database.pushFunction(type);

            Function toInt = new Function();
            toInt.Name = "toInt";
            toInt.agument.push("string", "context");
            toInt.call = new ToInt();
            database.pushFunction(toInt);
        }
    }

    class ToInt : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            string s = stack.pop().toString(new Posision(0, 0));
            double result;
            if(!double.TryParse(s, out result))
            {
                throw new ScriptError(s + " could not be convertet to int", new Posision(0, 0));
            }

            return new IntVariabel(result);
        }
    }

    class TypeControler : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            CVar var = stack.pop();
            string str = stack.pop().toString(new Posision(0,0));

            if (var is ObjectVariabel && str == ((ObjectVariabel)var).Name)
                return new BooleanVariabel(true);

            return new BooleanVariabel(var.type() == str);
        }
    }
}
