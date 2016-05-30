using script.builder;
using script.variabel;

namespace script.Container
{
    public class ClassItemContainer
    {
        public string Name { set; get; }
        public CVar Context { set; get; }
        public ClassItemAccessLevel Level { set; get; }
        public bool IsPointer { set; get; }
    }
}
