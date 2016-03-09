using script.variabel;
using System;

namespace script.Type
{
    class TypeHandler
    {
        public static ObjectVariabel ToObjectVariabel(CVar var)
        {
            return (ObjectVariabel)var;
        }

        public static bool controlType(CVar variabel, string type)
        {
            //both int and string got a valid method toString and int has toInt
            if (variabel.type() == "object")
            {
                //wee control the object typer (string and int)
                if (type == "string")
                {
                    return controlMethod((ObjectVariabel)variabel, "toString", 0);
                }
                else if (type == "int")
                {
                    return controlMethod((ObjectVariabel)variabel, "toInt", 0);
                }
            }

            if (variabel.type() == "object" && ((ObjectVariabel)variabel).Name == type)
                return true;

            //funtion can also be method and method funtion
            if (variabel.type() == "function" && type == "method" || variabel.type() == "method" && type == "function")
                return true;

            return variabel.type() == type;
        }

        public static bool isString(CVar variabel)
        {
            return variabel.type() == "object" && controlMethod((ObjectVariabel)variabel, "toString", 0);
        }

        public static bool isInt(CVar variabel)
        {
            return variabel.type() == "object" && controlMethod((ObjectVariabel)variabel, "toInt", 0);
        }

        private static bool controlMethod(ObjectVariabel obj, string name, int agumentSize)
        {
            if(!obj.containsItem(name))
            {
                return false;
            }

            //control the method size
            if (((MethodVariabel)obj.get(name)).agumentSize() != agumentSize)
            {
                return false;
            }

            return true;
        }
    }
}
