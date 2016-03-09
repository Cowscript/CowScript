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

        public void SetPrivate()
        {
            pointer.IsPublic = false;
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
                IsPublic = true,
                DefaultValue = new NullVariabel(),
            };
        }
    }
}
