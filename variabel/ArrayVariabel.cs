using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace script.variabel
{
    class ArrayVariabel : CVar
    {
        private int nextID = 0;
        private Dictionary<string, CVar> container = new Dictionary<string, CVar>();

        //okay let get the next id :)
        public int getNextID()
        {
            return nextID++;
        }

        public int length()
        {
            return container.Count;
        }

        //wee add one element in the array
        public void put(CVar key, CVar value, Posision pos)
        {
            //wee controlt he key
            controlID(key, pos);
            //wee control if wee got this id allready :)
            if (container.ContainsKey(key.toString(pos)))
                container.Remove(key.toString(pos));

            //wee add the key and value :)
            container.Add(key.toString(pos), value);
        }

        public bool keyExists(CVar key, Posision pos)
        {
            return container.ContainsKey(key.toString(pos));
        }

        public CVar get(CVar key, Posision pos)
        {
            if (!keyExists(key, pos))
                throw new ScriptError("Unknown key in array: " + key.toString(pos), pos);

            return container[key.toString(pos)];
        }

        public CVar get(string key, Posision pos)
        {
            return get(new StringVariabel(key), pos);
        }

        public override bool compare(CVar var, Posision pos)
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

        private void controlID(CVar key, Posision pos)
        {
            double k;

            if(key is IntVariabel)
            {
                k = key.toInt(pos);
            }else if(key is NullVariabel)
            {
                k = 0;
            }else if(key is StringVariabel && Regex.IsMatch(key.toString(pos), "^[0-9]*?$"))
            {
                k = Convert.ToDouble(key.toString(pos));
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
