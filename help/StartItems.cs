using script.builder;
using script.Type;
using script.variabel;
using System;
using System.Globalization;

namespace script.help
{
    class StartItems
    {
        public static void CreateStartItems(EnegyData data, VariabelDatabase db)
        {
            createStringClass(data, db, new Posision(0,0));
            createIntClass(data, db, new Posision(0,0));
        }

        private static void createIntClass(EnegyData data, VariabelDatabase db, Posision pos)
        {
            Class i = new Class("int");

            Method constructor = new Method(null);
            constructor.GetAgumentStack().push("int", "double", new NullVariabel());
            constructor.SetBody(Constructor_caller1);
            i.SetConstructor(constructor, data, db, pos);

            Method toInt = new Method("toInt");
            toInt.SetBody(ToInt_caller);
            i.SetMethod(toInt, data, db, pos);

            Method toString = new Method("toString");
            toString.SetBody(ToString_caller1);
            i.SetMethod(toString, data, db, pos);

            Method convert = new Method("convert");
            convert.SetStatic();
            convert.GetAgumentStack().push("string", "int");
            convert.SetBody(Convert_caller);
            i.SetMethod(convert, data, db, pos);

            db.pushClass(i, data);
        }

        private static CVar Constructor_caller1(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            TypeHandler.ToObjectVariabel(obj).systemItems.Add("int", (stack[0] is NullVariabel ? 0 : stack[0].toInt(pos, data, db)));
            return null;
        }

        private static CVar ToString_caller1(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toString(((double)TypeHandler.ToObjectVariabel(obj).systemItems["int"]).ToString(CultureInfo.GetCultureInfo("en-US")), data, db, pos);
        }

        private static CVar Convert_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            double number;

            if (double.TryParse(stack[0].toString(pos, data, db), out number))
            {
                return Types.toInt(number, data, db, pos);
            }

            return Types.toInt(0, data, db, pos);
        }

        private static CVar ToInt_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj;
        }

        private static void createStringClass(EnegyData data, VariabelDatabase db, Posision pos)
        {
            Class s = new Class("string");

            Method format = new Method("format");
            format.SetStatic();
            format.GetAgumentStack().push("string", "query");
            format.GetAgumentStack().push("array", "parameters");
            format.SetBody(Format_caller);
            s.SetMethod(format, data, db, pos);

            //constructor :)
            Method constructor = new Method(null);
            constructor.GetAgumentStack().push("string", "s", new NullVariabel());
            constructor.SetBody(Constructor_caller);
            s.SetConstructor(constructor, data, db, pos);

            Method length = new Method("length");
            length.SetBody(Length_caller);
            s.SetMethod(length, data, db, pos);

            Method substr = new Method("substr");
            substr.GetAgumentStack().push("int", "min");
            substr.GetAgumentStack().push("int", "max", new NullVariabel());
            substr.SetBody(Substr_caller);
            s.SetMethod(substr, data, db, pos);

            Method indexOf = new Method("indexOf");
            indexOf.GetAgumentStack().push("string", "serche");
            indexOf.SetBody(IndexOf_caller);
            s.SetMethod(indexOf, data, db, pos);

            Method toChars = new Method("toChars");
            toChars.SetBody(ToChars_caller);
            s.SetMethod(toChars, data, db, pos);

            Method split = new Method("split");
            split.GetAgumentStack().push("string", "string");
            split.SetBody(Split_caller);
            s.SetMethod(split, data, db, pos);

            Method toLower = new Method("toLower");
            toLower.SetBody(ToLower_caller);
            s.SetMethod(toLower, data, db, pos);

            Method toUpper = new Method("toUpper");
            toUpper.SetBody(ToUpper_caller);
            s.SetMethod(toUpper, data, db, pos);

            Method toString = new Method("toString");
            toString.SetBody(ToString_caller);
            s.SetMethod(toString, data, db, pos);

            Method set = new Method("set");
            set.GetAgumentStack().push("string", "context");
            set.setLevel(ClassItemAccessLevel.Protected);
            set.SetBody(Set_caller);
            s.SetMethod(set, data, db, pos);

            db.pushClass(s, data);
        }

        private static CVar Set_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            TypeHandler.ToObjectVariabel(c).systemItems["str"] = stack[0].toString(pos, data, db);
            return null;
        }

        private static CVar Format_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            StringFormater format = new StringFormater();
            ArrayVariabel array = (ArrayVariabel)stack[1];

            foreach(string key in array.Keys())
            {
                CVar context = array.get(key, pos, data, db);

                if (Types.instanceof((ClassVariabel)db.get("string", data), (ObjectVariabel)context))
                {
                    format.appendParam(context.toString(pos, data, db));
                }else if(Types.instanceof((ClassVariabel)db.get("int", data), (ObjectVariabel)context))
                {
                    format.appendParam(Convert.ToInt32(context.toInt(pos, data, db)));
                }
                else
                {
                    data.setError(new ScriptError("Cant convert '" + context.type() + "' to string", pos), db);
                }
            }

            return Types.toString(format.format(stack[0].toString(pos, data, db)), data, db, pos);
        }

        private static CVar ToString_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return obj;
        }

        private static CVar ToUpper_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toString(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).ToUpper(), data, db, pos);
        }

        private static CVar ToLower_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toString(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).ToLower(), data, db, pos);
        }

        private static CVar Split_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel(data, db, pos);

            foreach (string i in ((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).Split(new string[] { stack[0].toString(pos, data, db) }, StringSplitOptions.RemoveEmptyEntries))
            {
                array.put(array.getNextID(data, db, pos), Types.toString(i, data, db, pos), pos, data, db);
            }

            return array;
        }

        private static CVar ToChars_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel(data, db, pos);
            foreach (char c in ((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).ToCharArray())
                array.put(Types.toString(c.ToString(), data, db, pos), pos, data, db);

            return array;
        }

        private static CVar IndexOf_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).IndexOf(stack[0].toString(pos, data, db)), data, db, pos);
        }

        private static CVar Substr_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (stack[1] is NullVariabel)
            {
                return Types.toString(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).Substring((int)stack[0].toInt(pos, data, db)), data, db, pos);
            }

            return Types.toString(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).Substring((int)stack[0].toInt(pos, data, db), (int)stack[1].toInt(pos, data, db)), data, db, pos);
        }

        private static CVar Length_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((string)TypeHandler.ToObjectVariabel(obj).systemItems["str"]).Length, data, db, pos);
        }

        private static CVar Constructor_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            TypeHandler.ToObjectVariabel(obj).systemItems.Add("str", stack[0] is NullVariabel ? "" : stack[0].toString(pos, data, db));

            return null;
        }
    }
}
