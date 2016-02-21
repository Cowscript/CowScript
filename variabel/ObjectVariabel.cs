using script.help;
using script.Type;
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

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (StringVariabel.isString(this))
            {
                return StringVariabel.compare(this, var, pos, data, db);
            }else if (IntVariabel.isInt(this))
            {
                return IntVariabel.compare(this, var, data, db, pos);
            }

            return this == var;
        }

        public override string type()
        {
            return "object";
        }

        public bool containsItem(string name)
        {
            return items.ContainsKey(name);
        }

        public override double toInt(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (IntVariabel.isInt(this))
            {
                return (double)systemItems["int"];
            }

            if (!items.ContainsKey("toInt"))
            {
                return base.toInt(pos, data, db);
            }

            if (!items["toInt"].isPublic)
                return base.toInt(pos, data, db);

            if (!items["toInt"].isMethod)
                return base.toInt(pos, data, db);

            if (((MethodVariabel)items["toInt"].Context).agumentSize() < 0)
            {
                return base.toInt(pos, data, db);
            }

            return ((MethodVariabel)items["toInt"].Context).call(new CVar[0], db, data, pos).toInt(pos, data, db);
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (StringVariabel.isString(this))
            {
                //this is string variabel :)
                return (string)systemItems["str"];
            }

            if (!items.ContainsKey("toString"))
            {
                return base.toString(pos, data, db);
            }

            if (!items["toString"].isPublic)
                return base.toString(pos, data, db);

            if (!items["toString"].isMethod)
                return base.toString(pos, data, db);

            if (((MethodVariabel)items["toString"].Context).agumentSize() < 0)
            {
                return base.toString(pos, data, db);
            }

            return ((MethodVariabel)items["toString"].Context).call(new CVar[0], db, data, pos).toString(pos, data, db);
        }
    }
}
