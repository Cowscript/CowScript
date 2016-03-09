using System.Collections;
using System.Collections.Generic;

namespace script.Container
{
    public class ClassContainer
    {
        public string Name { get; set; } //the name of the class
        public Dictionary<string, MethodContainer> Methods { get; set; }
        public Dictionary<string, MethodContainer> StaticMethod { set; get; }
        public Dictionary<string, PointerContainer> Pointer { get; set; }
        public Dictionary<string, PointerContainer> StaticPointer { set; get; }
        public MethodContainer Constructor { set; get; }
        public VariabelDatabase ExtraVariabelDatabase { set; get; }
        public ArrayList Extends { set; get; }

    }
}
