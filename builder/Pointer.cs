using script.Container;
using script.variabel;

namespace script.builder
{
    public class Pointer
    {
        private PointerContainer pointer;

        public Pointer(string name)
        {
            pointer = Create(name);
        }

        public PointerContainer GetPointerContainer()
        {
            return pointer;
        }

        public void SetStatic()
        {
            pointer.IsStatic = true;
        }

        public void SetLevel(ClassItemAccessLevel level)
        {
            pointer.Level = level;
        }

        public void SetDefault(CVar defaultValue)
        {
            pointer.DefaultValue = defaultValue;
        }

        private PointerContainer Create(string name)
        {
            return new PointerContainer()
            {
                Name = name,
                IsStatic = false,
                Level = ClassItemAccessLevel.Public,
                DefaultValue = new NullVariabel(),
            };
        }
    }
}
