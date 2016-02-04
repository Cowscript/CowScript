using script.builder;
using script.variabel;
using System;

namespace script.help
{
    class StartItems
    {
        public static void CreateStartItems(EnegyData data, VariabelDatabase db)
        {
            createStringClass(data, db);
        }


        private static void createStringClass(EnegyData data, VariabelDatabase db)
        {
            Class s = new Class("string", data, db);

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
                array.put(array.getNextID(), StringVariabel.CreateString(data, db, pos, i), pos, data, db);
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
            return new IntVariabel(((string)obj.systemItems["str"]).IndexOf(stack[0].toString(pos, data, db)));
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
            return new IntVariabel(((string)obj.systemItems["str"]).Length);
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
