using script.variabel;

namespace script.Type
{
    class Types
    {
        public static ObjectVariabel toString(string context, EnegyData data, VariabelDatabase db, Posision pos)
        {
            ObjectVariabel obj = ((ClassVariabel)db.get("string", data)).createNew(db, data, new CVar[1] { new NullVariabel() }, pos);
            obj.systemItems["str"] = context;
            return obj;
        }

        public static ObjectVariabel toInt(double ints, EnegyData data, VariabelDatabase db, Posision pos)
        {
            ObjectVariabel obj = ((ClassVariabel)db.get("int", data)).createNew(db, data, new CVar[1] { new NullVariabel() }, pos);
            obj.systemItems["int"] = ints;
            return obj;
        }

        public static bool instanceof(ClassVariabel c, ObjectVariabel obj)
        {
            if (c.Name == obj.Name)
                return true;//direct control is success :)

            foreach (ClassVariabel extends in obj.GetExtends)
            {
                if (c.Name == extends.Name)
                    return true;
            }

            return false;
        }

        public static bool IsType(string control, VariabelDatabase db)
        {
            if(control.IndexOf("function") == 0)
            {
                if (control == "function")
                    return true;

                if(control.Substring(8,1) == "<" && control.Substring(control.Length-1, 1) == ">")
                {
                    string buffer = control.Substring(8);
                    return IsType(buffer.Substring(0, buffer.Length - 1), db);
                }
            }

            return db.isType(control);
        }
    }
}
