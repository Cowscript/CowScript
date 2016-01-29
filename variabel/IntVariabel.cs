using System.Globalization;

namespace script.variabel
{
    public class IntVariabel : CVar
    {
        private double context;

        public IntVariabel(double d)
        {
            context = d;
        }

        public override double toInt(Posision pos, EnegyData data, VariabelDatabase db)
        {
            return context;
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            return context.ToString(CultureInfo.GetCultureInfo("en-US"));
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return var.type() == type() && var.toInt(pos, data, db) == toInt(pos, data, db);
        }

        public override string type()
        {
            return "int";
        }
    }
}
