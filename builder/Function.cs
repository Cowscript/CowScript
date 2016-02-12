using script.help;
using script.stack;
using script.variabel;

namespace script.builder
{
    public class Function
    {
        public delegate CVar CallFunctionEvent(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos);
        public string Name { get; set; }
        public AgumentStack agument = new AgumentStack();
        public event CallFunctionEvent call;
        public VariabelDatabase extraVariabelDatabase { get; set; }
        public string ReturnType { get; set; }

        public CVar callFunction(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            if (call == null)
            {
                return new NullVariabel();
            }

            return call(stack, db, data, pos);
        }
    }
}
