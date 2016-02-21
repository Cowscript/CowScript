using System.Globalization;

namespace script.variabel
{
    public class IntVariabel
    {
        public static ObjectVariabel createInt(EnegyData data, VariabelDatabase db, Posision posision, double context)
        {
            ObjectVariabel obj = ((ClassVariabel)db.get("int", data)).createNew(db, data, posision);
            obj.systemItems["int"] = context;
            return obj;
        }

        public static bool isInt(CVar item)
        {
            return item.type() == "object" && ((ObjectVariabel)item).Name == "int";
        }

        public static bool compare(CVar one, CVar two, EnegyData data, VariabelDatabase db, Posision pos)
        {
            return isInt(one) &&
                   isInt(two) &&
                   ((ObjectVariabel)one).toInt(pos, data, db) == ((ObjectVariabel)two).toInt(pos, data, db);
        }
    }
}
