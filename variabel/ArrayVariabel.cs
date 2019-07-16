using script.builder;
using script.Container;
using script.Type;
using System;
using System.Collections.Generic;

namespace script.variabel
{
    class ArrayVariabel : ObjectVariabel
    {
        private int nextID = 0;
        private Dictionary<string, CVar> container = new Dictionary<string, CVar>();

        public ArrayVariabel(EnegyData data, VariabelDatabase db, Posision pos) : base(new ClassVariabel(new Class()), new Dictionary<string, PointerContainer>(), new Dictionary<string, MethodContainer>(), null, new System.Collections.ArrayList())
        {
            Class c = new Class("Array");

            Method length = new Method("length");
            length.SetBody(Length_caller);
            c.SetMethod(length, data, db, pos);

            Method hasValue = new Method("hasValue");
            hasValue.GetAgumentStack().push("context");
            hasValue.SetBody(HasValue_caller);
            c.SetMethod(hasValue, data, db, pos);

            Method hasKey = new Method("hasKey");
            hasKey.GetAgumentStack().push("string", "context");
            hasKey.SetBody(HasKey_caller);
            c.SetMethod(hasKey, data, db, pos);

            Method removeKey = new Method("removeKey");
            removeKey.GetAgumentStack().push("string", "key");
            removeKey.SetBody(RemoveKey_caller);
            c.SetMethod(removeKey, data, db, pos);

            Method removeValue = new Method("removeValue");
            removeValue.GetAgumentStack().push("value");
            removeValue.SetBody(RemoveValue_caller);
            c.SetMethod(removeValue, data, db, pos);

            ClassVariabel i = new ClassVariabel(c);
            ObjectVariabel o = i.createNew(db, data, new CVar[0], pos);
            Items = o.Items;
        }

        private CVar RemoveValue_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            foreach(string key in container.Keys)
            {
                if(container[key].compare(stack[0], pos, data, db))
                {
                    container.Remove(key);
                    return new BooleanVariabel(true);
                }
            }

            return new BooleanVariabel(false);
        }

        private CVar RemoveKey_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            string key = stack[0].toString(pos, data, db);

            if (!container.ContainsKey(key))
                return new BooleanVariabel(false);

            container.Remove(key);
            return new BooleanVariabel(true);
        }

        private CVar HasKey_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return new BooleanVariabel(keyExists(stack[0], pos, data, db));
        }

        private CVar HasValue_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            foreach(string key in Keys())
            {
                if (get(key, pos, data, db).compare(stack[0], pos, data, db))
                    return new BooleanVariabel(true);
            }

            return new BooleanVariabel(false);
        }

        private CVar Length_caller(CVar obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            return Types.toInt(container.Count, data, db, pos);
        }

        //okay let get the next id :)
        public ObjectVariabel getNextID(EnegyData data, VariabelDatabase db, Posision posision)
        {
            return Types.toInt((double)nextID++, data, db, posision);
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
            return get(Types.toString(key, data, db, pos), pos, data, db);
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

            if(Types.instanceof((ClassVariabel)db.get("int", data), (ObjectVariabel)key))
            {
                k = key.toInt(pos, data, db);
            }else if(key is NullVariabel)
            {
                k = 0;
            }else if(Types.instanceof((ClassVariabel)db.get("string", data), (ObjectVariabel)key) && System.Text.RegularExpressions.Regex.IsMatch(key.toString(pos, data, db), "^[0-9]*?$"))
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
