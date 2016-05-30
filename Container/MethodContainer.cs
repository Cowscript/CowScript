using script.builder;
using script.stack;
using script.variabel;

namespace script.Container
{
    public class MethodContainer
    {
        public delegate CVar MethodDelegate(CVar owner, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos);

        public string Name { set; get; }
        public bool IsStatic { set; get; }
        public ClassItemAccessLevel Level { set; get; }
        public AgumentStack Agument { set; get; }
        public MethodDelegate Body { set; get; }
        public bool SetVariabel { set; get; }
        public string ReturnType { set; get; }
    }
}
