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
            ClassMethods length = (ClassMethods)c.createMethods();
            length.setName("length");
            length.setAccess(true);
            length.caller += Length_caller;
            length.create();

            ClassMethods substr = (ClassMethods)c.createMethods();
            substr.setName("substr");
            substr.setAccess(true);
            substr.caller += Substr_caller;

            //add aguments :)
            AgumentStack substrStack = new AgumentStack();
            substrStack.push("int", "min");
            substrStack.push("int", "max", new NullVariabel());
            substr.Aguments = substrStack;

            substr.create();

            ClassMethods indexOf = (ClassMethods)c.createMethods();
            indexOf.setName("indexOf");
            indexOf.setAccess(true);
            indexOf.caller += IndexOf_caller;

            AgumentStack indexOfStack = new AgumentStack();
            indexOfStack.push("string", "serche");
            indexOf.Aguments = indexOfStack;

            indexOf.create();

            ClassMethods toChars = (ClassMethods)c.createMethods();
            toChars.setName("toChars");
            toChars.setAccess(true);
            toChars.caller += ToChars_caller;
            toChars.create();

            ClassMethods split = (ClassMethods)c.createMethods();
            split.setName("split");
            split.setAccess(true);
            AgumentStack splitStack = new AgumentStack();
            splitStack.push("string", "string");
            split.Aguments = splitStack;
            split.caller += Split_caller;
            split.create();

            ClassVariabel cVar = new ClassVariabel(c);
            items = cVar.createNew(new CallAgumentStack(), new VariabelDatabase(), new EnegyData(new VariabelDatabase(), new Interprenter(), new PluginContainer(), null, null)).items;
        }

        private CVar Split_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            ArrayVariabel array = new ArrayVariabel();

            foreach(string i in context.Split(new string[] { stack.pop().toString(new Posision(0, 0)) }, StringSplitOptions.RemoveEmptyEntries))
            {
                array.put(new IntVariabel(array.getNextID()), new StringVariabel(i), new Posision(0, 0));
            }

            return array;
        }

        private CVar ToChars_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            ArrayVariabel array = new ArrayVariabel();
            foreach (char c in context.ToCharArray())
                array.put(new IntVariabel(array.getNextID()), new StringVariabel(c.ToString()), new Posision(0,0));

            return array;
        }

        private CVar IndexOf_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(context.IndexOf(stack.pop().toString(new Posision(0, 0))));
        }

        private CVar Substr_caller(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            double start = stack.pop().toInt(new Posision(0,0));
            CVar end = stack.pop();

            if(end is NullVariabel)
            {
                return new StringVariabel(context.Substring((int)start));
            }

            return new StringVariabel(context.Substring((int)start, (int)end.toInt(new Posision(0,0))));
        }

        private CVar Length_caller(ObjectVariabel obj, VariabelDatabase db, stack.CallAgumentStack stack, EnegyData data)
        {
            return new IntVariabel(context.Length);
        }

        public override string toString(Posision pos)
        {
            return context;
        }

        public override string type()
        {
            return "string";
        }

        public override bool compare(CVar var, Posision pos)
        {
            return var.type() == type() && toString(pos) == var.toString(pos);
        }
    }
}
