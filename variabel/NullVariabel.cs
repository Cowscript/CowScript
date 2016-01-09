﻿namespace script.variabel
{
    public class NullVariabel : CVar
    {
        public override bool toBoolean(Posision pos)
        {
            return false;
        }

        public override string toString(Posision pos)
        {
            return "";
        }

        public override double toInt(Posision pos)
        {
            return 0;
        }

        public override bool compare(CVar var, Posision pos)
        {
            return var.type() == type();
        }

        public override string type()
        {
            return "null";
        }
    }
}
