using script.builder;
using System;
using System.Collections.Generic;

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

            ClassMethods hasValue = new ClassMethods(c, "hasValue");
            hasValue.Aguments.push("context");
            hasValue.caller += HasValue_caller;
            hasValue.create();

            ClassMethods hasKey = new ClassMethods(c, "hasKey");
            hasKey.Aguments.push("context");
            hasKey.caller += HasKey_caller;
            hasKey.create();

            items = new ClassVariabel(c).createNew(new CVar[0], new VariabelDatabase(), new EnegyData(), new Posision(0,0)).items;
        }

        private CVar HasKey_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(keyExists(stack[0], pos, data, db));
        }

        private CVar HasValue_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            foreach(string key in Keys())
            {
                if (get(key, pos, data, db).compare(stack[0], pos, data, db))
                    return new BooleanVariabel(true);
            }

            return new BooleanVariabel(false);
        }

        private CVar Length_caller(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return IntVariabel.createInt(data, db, pos, container.Count);
        }

        //okay let get the next id :)
        public ObjectVariabel getNextID(EnegyData data, VariabelDatabase db, Posision posision)
        {
            return IntVariabel.createInt(data, db, posision, nextID++);
        }

        public int length()
        {
            return container.Count;
        }

        public void put(CVar value, Posision pos, EnegyData data, VariabelDatabase db)
        {
            container.Add(getNextID(data, db, pos).toString(pos, data, db), value);
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
            {
                data.setError(new ScriptError("Unknown key in array: " + key.toString(pos, data, db), pos), db);
                return new NullVariabel();
            }

            return container[key.toString(pos, data, db)];
        }

        public CVar get(string key, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return get(StringVariabel.CreateString(data, db, pos, key), pos, data, db);
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

            if(IntVariabel.isInt(key))
            {
                k = key.toInt(pos, data, db);
            }else if(key is NullVariabel)
            {
                k = 0;
            }else if(StringVariabel.isString(key) && System.Text.RegularExpressions.Regex.IsMatch(key.toString(pos, data, db), "^[0-9]*?$"))
            {
                k = Convert.ToDouble(key.toString(pos, data, db));
            }
            else
            {
                return;
            }


            while (k >= nextID)
                getNextID(data, db, pos);
        }
    }
}
