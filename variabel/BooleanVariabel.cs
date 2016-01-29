namespace script.variabel
{
    public class BooleanVariabel : CVar
    {
        private bool context;

        public BooleanVariabel(bool context)
        {
            this.context = context;
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return var.type() == type() && var.toBoolean(pos, data, db) == toBoolean(pos, data, db);//here wee control if it is same as this
        }

        public override string type()
        {
            return "bool";
        }

        public override bool toBoolean(Posision pos, EnegyData data, VariabelDatabase db)
        {
            return context;
        }
    }
}
