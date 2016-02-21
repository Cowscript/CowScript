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
                return StringVariabel.CreateString(data, db, new Posision(0,0), (string)c);
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
                return IntVariabel.createInt(data, db, new Posision(0,0), (double)c);
            }
            else if (c is int)
            {
                return IntVariabel.createInt(data, db, new Posision(0, 0), Convert.ToDouble((int)c));
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
