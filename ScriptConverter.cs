using script.variabel;
using System;

namespace script
{
    class ScriptConverter
    {
        public static CVar convert(object c)
        {
            if (c is string)
            {
                return new StringVariabel((string)c);
            }
            else if (c is bool)
            {
                return new BooleanVariabel((bool)c);
            }
            else if (c is FunctionVariabel)
            {
                return (FunctionVariabel)c;
            }
            else if (c is double)
            {
                return new IntVariabel((double)c);
            }
            else if (c is int)
            {
                return new IntVariabel(Convert.ToDouble((int)c));
            }
            else if (c is ObjectVariabel)
                return (ObjectVariabel)c;
            else if(c is ClassVariabel)
            {
                return (ClassVariabel)c;
            }

            throw new ScriptError("Unkown type: " + c.GetType().Name, new Posision(0, 0));
        }
    }
}
