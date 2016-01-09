namespace script.variabel
{
    public class BooleanVariabel : CVar
    {
        private bool context;

        public BooleanVariabel(bool context)
        {
            this.context = context;
        }

        public override bool compare(CVar var, Posision pos)
        {
            return var.type() == type() && var.toBoolean(pos) == toBoolean(pos);//here wee control if it is same as this
        }

        public override string type()
        {
            return "bool";
        }

        public override bool toBoolean(Posision pos)
        {
            return context;
        }
    }
}
