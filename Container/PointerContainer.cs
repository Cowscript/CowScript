using script.builder;
using script.variabel;

namespace script.Container
{
    public class PointerContainer
    {
        public string Name { set; get; }
        public bool IsStatic { set; get; }
        public ClassItemAccessLevel Level { set; get; }
        public CVar DefaultValue { set; get; }
    }
}
