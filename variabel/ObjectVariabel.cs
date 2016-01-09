using script.help;
using System.Collections.Generic;

namespace script.variabel
{
    public class ObjectVariabel : CVar
    {
        public Dictionary<string, ObjectItemData> items = new Dictionary<string, ObjectItemData>();
        public Dictionary<string, object> systemItems = new Dictionary<string, object>();

        public string Name { private set; get; }

        public ObjectVariabel(string className) { Name = className; }

        public void put(ObjectItemData data)
        {
            items.Add(data.Name, data);
        }

        public CVar get(string name)
        {
            return items[name].Context;
        }

        public bool isPointer(string name)
        {
            return !items[name].isMethod;
        }

        public bool isPublic(string name)
        {
            return items[name].isPublic;
        }

        public void appendToPointer(string name, CVar value)
        {
            if (!containsItem(name) || !isPointer(name))
                return;

            items[name].Context = value;
        }

        public override bool compare(CVar var, Posision pos)
        {
            throw new ScriptError("Compara to object is not suppoerted yet", pos);
        }

        public override string type()
        {
            return "object";
        }

        public bool containsItem(string name)
        {
            return items.ContainsKey(name);
        }
    }
}
