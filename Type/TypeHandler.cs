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
            
            //control if the type is function
            if(type.IndexOf("function") == 0)
            {
                //control if the hole type contain 'function'
                if(type == "function")
                {
                    return variabel.type() == "function" || variabel.type() == "method";
                }
                
                if (type.Substring(8, 1) != "<" || type.Substring(type.Length-1) != ">")
                    return false;
                string rt = type.Substring(9, type.Length - 10);
                return variabel.type() == "function" && ((FunctionVariabel)variabel).func.ReturnType == rt || variabel.type() == "method" && ((MethodVariabel)variabel).method.ReturnType == rt;
            }

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
