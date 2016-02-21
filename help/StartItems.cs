using script.builder;
using script.variabel;
using System;
using System.Globalization;

namespace script.help
{
    class StartItems
    {
        public static void CreateStartItems(EnegyData data, VariabelDatabase db)
        {
            createStringClass(data, db);
            createIntClass(data, db);
        }

        private static void createIntClass(EnegyData data, VariabelDatabase db)
        {
            Class i = new Class("int", data, db);

            ClassMethods constructor = new ClassMethods(i, "");
            constructor.Aguments.push("int", "double", new NullVariabel());
            constructor.caller += Constructor_caller1;
            constructor.createConstructor();

            ClassMethods toInt = new ClassMethods(i, "toInt", true, "int");
            toInt.caller += ToInt_caller;
            toInt.create();

            ClassMethods toString = new ClassMethods(i, "toString", true, "string");
            toString.caller += ToString_caller1;
            toString.create();

            ClassStaticMethods convert = new ClassStaticMethods(i, "convert", true, "int");
            convert.Aguments.push("string", "int");
            convert.caller += Convert_caller;
            convert.create();

            db.pushClass(i, data);
        }

        private static CVar Constructor_caller1(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (stack[0] is NullVariabel)
                obj.systemItems.Add("int", 0);
            else
                obj.systemItems.Add("int", stack[0].toInt(pos, data, db));
            return null;
        }

        private static CVar ToString_caller1(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return StringVariabel.CreateString(data, db, pos, ((double)obj.systemItems["int"]).ToString(CultureInfo.GetCultureInfo("en-US")));
        }

        private static CVar Convert_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            double number;

            if (double.TryParse(stack[0].toString(pos, data, db), out number))
            {
                return IntVariabel.createInt(data, db, pos, number);
            }

            return IntVariabel.createInt(data, db, pos, 0);
        }

        private static CVar ToInt_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj;
        }

        private static void createStringClass(EnegyData data, VariabelDatabase db)
        {
            Class s = new Class("string", data, db);

            ClassStaticMethods format = new ClassStaticMethods(s, "format");
            format.Aguments.push("string", "query");
            format.Aguments.push("array", "parameters");
            format.caller += Format_caller;
            format.create();

            //constructor :)
            ClassMethods constructor = new ClassMethods(s, "constructor");
            constructor.Aguments.push("string", "s", new NullVariabel());
            constructor.caller += Constructor_caller;
            constructor.createConstructor();

            ClassMethods length = new ClassMethods(s, "length");
            length.caller += Length_caller;
            length.create();

            ClassMethods substr = new ClassMethods(s, "substr");
            substr.Aguments.push("int", "min");
            substr.Aguments.push("int", "max", new NullVariabel());
            substr.caller += Substr_caller;
            substr.create();

            ClassMethods indexOf = new ClassMethods(s, "indexOf");
            indexOf.caller += IndexOf_caller;
            indexOf.Aguments.push("string", "serche");
            indexOf.create();

            ClassMethods toChars = new ClassMethods(s, "toChars");
            toChars.caller += ToChars_caller;
            toChars.create();

            ClassMethods split = new ClassMethods(s, "split");
            split.Aguments.push("string", "string");
            split.caller += Split_caller; ;
            split.create();

            ClassMethods toLower = new ClassMethods(s, "toLower");
            toLower.caller += ToLower_caller; ;
            toLower.create();

            ClassMethods toUpper = new ClassMethods(s, "toUpper");
            toUpper.caller += ToUpper_caller; ;
            toUpper.create();

            ClassMethods toString = new ClassMethods(s, "toString");
            toString.caller += ToString_caller;
            toString.create();

            db.pushClass(s, data);
        }

        private static CVar Format_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            StringFormater format = new StringFormater();
            ArrayVariabel array = (ArrayVariabel)stack[1];

            foreach(string key in array.Keys())
            {
                CVar context = array.get(key, pos, data, db);

                if (StringVariabel.isString(context))
                {
                    format.appendParam(context.toString(pos, data, db));
                }else if(IntVariabel.isInt(context))
                {
                    format.appendParam(Convert.ToInt32(context.toInt(pos, data, db)));
                }
                else
                {
                    data.setError(new ScriptError("Cant convert '" + context.type() + "' to string", pos), db);
                }
            }

            return StringVariabel.CreateString(data, db, pos, format.format(stack[0].toString(pos, data, db)));
        }

        private static CVar ToString_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj;
        }

        private static CVar ToUpper_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return StringVariabel.CreateString(data, db, pos, ((string)obj.systemItems["str"]).ToUpper());
        }

        private static CVar ToLower_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return StringVariabel.CreateString(data, db, pos, ((string)obj.systemItems["str"]).ToLower());
        }

        private static CVar Split_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel();

            foreach (string i in ((string)obj.systemItems["str"]).Split(new string[] { stack[0].toString(pos, data, db) }, StringSplitOptions.RemoveEmptyEntries))
            {
                array.put(array.getNextID(data, db, pos), StringVariabel.CreateString(data, db, pos, i), pos, data, db);
            }

            return array;
        }

        private static CVar ToChars_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel();
            foreach (char c in ((string)obj.systemItems["str"]).ToCharArray())
                array.put(StringVariabel.CreateString(data, db, pos, c.ToString()), pos, data, db);

            return array;
        }

        private static CVar IndexOf_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, ((string)obj.systemItems["str"]).IndexOf(stack[0].toString(pos, data, db)));
        }

        private static CVar Substr_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (stack[1] is NullVariabel)
            {
                return StringVariabel.CreateString(data, db, pos, ((string)obj.systemItems["str"]).Substring((int)stack[0].toInt(pos, data, db)));
            }

            return StringVariabel.CreateString(data, db, pos, ((string)obj.systemItems["str"]).Substring((int)stack[0].toInt(pos, data, db), (int)stack[1].toInt(pos, data, db)));
        }

        private static CVar Length_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, ((string)obj.systemItems["str"]).Length);
        }

        private static CVar Constructor_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (stack[0] is NullVariabel)
            {
                obj.systemItems.Add("str", "");
            }
            else
                obj.systemItems.Add("str", stack[0].toString(pos, data, db));

            return null;
        }
    }
}
