using script.Container;

namespace script.variabel
{
    public class PointerVariabel : CVar
    {
        private PointerContainer pointer;

        public PointerVariabel(PointerContainer pointer)
        {
            this.pointer = pointer;
        }

        public bool IsPublic
        {
            get
            {
                return pointer.Level == builder.ClassItemAccessLevel.Public;
            }
        }

        public void setValue(CVar value)
        {
            pointer.DefaultValue = value;
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return pointer.DefaultValue.compare(var, pos, data, db);
        }

        public CVar getValue()
        {
            return pointer.DefaultValue;
        }

        public override string type()
        {
            return "pointer";
        }
    }
}
