using script.Type;
using script.variabel;
using System;

namespace script
{
    class ScriptConverter
    {
        public static CVar convert(object c, EnegyData data, VariabelDatabase db)
        {
            if (c is string)
            {
                return Types.toString((string)c, data, db, new Posision(0,0));
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
                return Types.toInt((double)c, data, db, new Posision(0,0));
            }
            else if (c is int)
            {
                return Types.toInt(Convert.ToDouble((int)c), data, db, new Posision(0,0));
            }
            else if (c is ObjectVariabel)
                return (ObjectVariabel)c;
            else if(c is ClassVariabel)
            {
                return (ClassVariabel)c;
            }

            data.setError(new ScriptError("Unkown type: " + c.GetType().Name, new Posision(0, 0)), db);
            return new NullVariabel();
        }
    }
}
