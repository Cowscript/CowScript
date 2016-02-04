using script.variabel;

namespace script.help
{
    public class ObjectItemData
    {
        public bool isPublic { set; get; }
        public string Name { set; get; }
        public CVar Context { set; get; }
        public bool isMethod { set; get; }
        public bool isStatic { set; get; }
        public VariabelDatabase extraVariabelDatabase { set; get; }
    }
}
