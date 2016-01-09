namespace script.variabel
{
    public class IntVariabel : CVar
    {
        private double context;

        public IntVariabel(double d)
        {
            context = d;
        }

        public override double toInt(Posision pos)
        {
            return context;
        }

        public override string toString(Posision pos)
        {
            return context.ToString();
        }

        public override bool compare(CVar var, Posision pos)
        {
            return var.type() == type() && var.toInt(pos) == toInt(pos);
        }

        public override string type()
        {
            return "int";
        }
    }
}
