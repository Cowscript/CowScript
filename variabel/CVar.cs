namespace script.variabel
{
    public abstract class CVar
    {
        public abstract bool compare(CVar var, Posision pos);
        public abstract string type();

        public virtual bool toBoolean(Posision pos)
        {
            throw new ScriptError("Can`t convert " + this.type() + " to Boolean", pos);
        }

        public virtual double toInt(Posision pos)
        {
            throw new ScriptError("Can`t convert " + this.type() + " to int", pos);
        }

        public virtual string toString(Posision pos)
        {
            throw new ScriptError("Can`t convert " + type() + " to string", pos);
        }
    }
}
