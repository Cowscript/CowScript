using script.builder;
using script.variabel;

namespace script.help
{
    public class ClassItems
    {
        public bool IsMethod { get; set; }
        public ClassItemsMethod Method { get; set; }
        public string Name { get; set; }
        public CVar Context { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
        public VariabelDatabase extraVariabelDatabase { get; set; }
    }
}
