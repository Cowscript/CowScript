namespace script.variabel
{
    public abstract class CVar
    {
        public abstract bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db);
        public abstract string type();

        public virtual bool toBoolean(Posision pos, EnegyData data, VariabelDatabase db)
        {
            data.setError(new ScriptError("Can`t convert " + this.type() + " to Boolean", pos), db);
            return false;
        }

        public virtual double toInt(Posision pos, EnegyData data, VariabelDatabase db)
        {
            data.setError(new ScriptError("Can`t convert " + this.type() + " to int", pos), db);
            return 0;
        }

        public virtual string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            data.setError(new ScriptError("Can`t convert " + type() + " to string", pos), db);
            return "";
        }
    }
}
