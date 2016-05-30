using script.builder;
using script.Type;
using script.variabel;
using System;

namespace script.plugin
{
    class Random : PluginInterface
    {
        public string Name { get { return "random"; } }

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class r = new Class("Random");

            Method constructor = new Method("");
            constructor.SetBody(Constructor_caller);
            r.SetConstructor(constructor, data, database, pos);

            Method seed = new Method("seed");
            seed.GetAgumentStack().push("int", "seed");
            seed.SetBody(Seed_caller);
            r.SetMethod(seed, data, database, pos);

            Method next = new Method("next");
            next.GetAgumentStack().push("int", "min");
            next.GetAgumentStack().push("int", "max");
            next.SetBody(Next_caller);
            r.SetMethod(next, data, database, pos);

            database.pushClass(r, data);
        }

        private CVar Next_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(((System.Random)TypeHandler.ToObjectVariabel(obj).systemItems["rand"]).Next(Convert.ToInt32(stack[0].toInt(pos, data, db)), Convert.ToInt32(stack[1].toInt(pos, data, db))), data, db, pos);
        }

        private CVar Seed_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            TypeHandler.ToObjectVariabel(obj).systemItems.Remove("rand");
            TypeHandler.ToObjectVariabel(obj).systemItems.Add("rand", new System.Random(Convert.ToInt32(stack[0].toInt(pos, data, db))));
            return null;
        }

        private CVar Constructor_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            TypeHandler.ToObjectVariabel(obj).systemItems.Add("rand", new System.Random());
            return null;
        }
    }
}
