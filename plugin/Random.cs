using script.builder;
using script.variabel;
using System;

namespace script.plugin
{
    class Random : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Class r = new Class("Random");

            ClassMethods constructor = new ClassMethods(r, "");
            constructor.caller += Constructor_caller;
            constructor.create();

            ClassMethods seed = new ClassMethods(r, "seed");
            seed.Aguments.push("int", "seed");
            seed.caller += Seed_caller;
            seed.create();

            ClassMethods next = new ClassMethods(r, "next");
            next.Aguments.push("int", "min");
            next.Aguments.push("int", "max");
            next.caller += Next_caller;
            next.create();

            database.pushClass(r, data);
        }

        private variabel.CVar Next_caller(variabel.ObjectVariabel obj, VariabelDatabase db, variabel.CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(((System.Random)obj.systemItems["rand"]).Next(Convert.ToInt32(stack[0].toInt(pos, data, db)), Convert.ToInt32(stack[1].toInt(pos, data, db))));
        }

        private variabel.CVar Seed_caller(variabel.ObjectVariabel obj, VariabelDatabase db, variabel.CVar[] stack, EnegyData data, Posision pos)
        {
            obj.systemItems.Remove("rand");
            obj.systemItems.Add("rand", new System.Random(Convert.ToInt32(stack[0].toInt(pos, data, db))));
            return null;
        }

        private variabel.CVar Constructor_caller(variabel.ObjectVariabel obj, VariabelDatabase db, variabel.CVar[] stack, EnegyData data, Posision pos)
        {
            obj.systemItems.Add("rand", new System.Random());
            return null;
        }
    }
}
