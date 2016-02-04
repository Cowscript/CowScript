using System;

namespace script.variabel
{
    public class StringVariabel
    {
        public static CVar CreateString(EnegyData data, VariabelDatabase db, Posision posision, string context)
        {
            ObjectVariabel obj = ((ClassVariabel)db.get("string", data)).createNew(db, data, posision);
            obj.systemItems["str"] = context;
            return obj;
        }

        public static bool isString(CVar item)
        {
            return item.type() == "object" && ((ObjectVariabel)item).Name == "string";
        }

        public static bool compare(CVar one, CVar two, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return isString(one) && isString(two) && ((ObjectVariabel)one).toString(pos, data, db) == ((ObjectVariabel)two).toString(pos, data, db);
        }
    }
}
