using script.builder;
using script.plugin;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace script.variabel
{
    class ArrayVariabel : ObjectVariabel
    {
        private int nextID = 0;
        private Dictionary<string, CVar> container = new Dictionary<string, CVar>();

        public ArrayVariabel() : base("array")
        {
            Class c = new Class("array");

            ClassMethods length = new ClassMethods(c, "length");
            length.caller += Length_caller;
            length.create();

            ClassVariabel cv = new ClassVariabel(c);
            items = cv.createNew(new CVar[0], new VariabelDatabase(), new EnegyData(), new Posision(0, 0)).items;
        }

        private CVar Length_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new IntVariabel(container.Count);
        }

        //okay let get the next id :)
        public IntVariabel getNextID()
        {
            return new IntVariabel(nextID++);
        }

        public int length()
        {
            return container.Count;
        }

        public void put(CVar value, Posision pos, EnegyData data, VariabelDatabase db)
        {
            container.Add(getNextID().toString(pos, data, db), value);
        }

        //wee add one element in the array
        public void put(CVar key, CVar value, Posision pos, EnegyData data, VariabelDatabase db)
        {
            //wee controlt he key
            controlID(key, pos, data, db);
            //wee control if wee got this id allready :)
            if (container.ContainsKey(key.toString(pos, data, db)))
                container.Remove(key.toString(pos, data, db));

            //wee add the key and value :)
            container.Add(key.toString(pos, data, db), value);
        }

        public bool keyExists(CVar key, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return container.ContainsKey(key.toString(pos, data, db));
        }

        public CVar get(CVar key, Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (!keyExists(key, pos, data, db))
                throw new ScriptError("Unknown key in array: " + key.toString(pos, data, db), pos);

            return container[key.toString(pos, data, db)];
        }

        public CVar get(string key, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return get(new StringVariabel(key), pos, data, db);
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return false;
        }

        public override string type()
        {
            return "array";
        }

        public Dictionary<string, CVar>.KeyCollection Keys()
        {
            return container.Keys;
        }

        private void controlID(CVar key, Posision pos, EnegyData data, VariabelDatabase db)
        {
            double k;

            if(key is IntVariabel)
            {
                k = key.toInt(pos);
            }else if(key is NullVariabel)
            {
                k = 0;
            }else if(key is StringVariabel && System.Text.RegularExpressions.Regex.IsMatch(key.toString(pos, data, db), "^[0-9]*?$"))
            {
                k = Convert.ToDouble(key.toString(pos, data, db));
            }
            else
            {
                return;
            }


            while (k >= nextID)
                getNextID();
        }
    }
}
