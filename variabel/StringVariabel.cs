using script.builder;
using script.plugin;
using script.stack;
using System;

namespace script.variabel
{
    public class StringVariabel : ObjectVariabel
    {
        private string context;

        public StringVariabel(string context) : base("string")
        {
            this.context = context;

            Class c = new Class("string");
            ClassMethods length = new ClassMethods(c, "length");
            length.caller += Length_caller;
            length.create();

            ClassMethods substr = new ClassMethods(c, "substr");
            substr.caller += Substr_caller;
            substr.Aguments.push("int", "min");
            substr.Aguments.push("int", "max", new NullVariabel());
            substr.create();

            ClassMethods indexOf = new ClassMethods(c, "indexOf");
            indexOf.caller += IndexOf_caller;
            indexOf.Aguments.push("string", "serche");
            indexOf.create();

            ClassMethods toChars = new ClassMethods(c, "toChars");
            toChars.caller += ToChars_caller;
            toChars.create();

            ClassMethods split = new ClassMethods(c, "split");
            split.Aguments.push("string", "string");
            split.caller += Split_caller;
            split.create();

            ClassMethods toLower = new ClassMethods(c, "toLower");
            toLower.caller += ToLower_caller;
            toLower.create();

            ClassMethods toUpper = new ClassMethods(c, "toUpper");
            toUpper.caller += ToUpper_caller;
            toUpper.create();

            ClassVariabel cVar = new ClassVariabel(c);
            items = cVar.createNew(new CVar[0], new VariabelDatabase(), new EnegyData(), new Posision(0,0)).items;
        }

        private CVar ToUpper_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new StringVariabel(context.ToUpper());
        }

        private CVar ToLower_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new StringVariabel(context.ToLower());
        }

        private CVar Split_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel();

            foreach(string i in context.Split(new string[] { stack[0].toString(pos, data, db) }, StringSplitOptions.RemoveEmptyEntries))
            {
                array.put(array.getNextID(), new StringVariabel(i), pos, data, db);
            }

            return array;
        }

        private CVar ToChars_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            ArrayVariabel array = new ArrayVariabel();
            foreach (char c in context.ToCharArray())
                array.put(array.getNextID(), new StringVariabel(c.ToString()), pos, data, db);

            return array;
        }

        private CVar IndexOf_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(context.IndexOf(stack[0].toString(pos, data, db)));
        }

        private CVar Substr_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            if (stack[1] is NullVariabel)
            {
                return new StringVariabel(context.Substring((int)stack[0].toInt(pos)));
            }

            return new StringVariabel(context.Substring((int)stack[0].toInt(pos), (int)stack[1].toInt(pos)));
        }

        private CVar Length_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(context.Length);
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            return context;
        }

        public override string type()
        {
            return "string";
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return var.type() == type() && toString(pos, data, db) == var.toString(pos, data, db);
        }
    }
}
