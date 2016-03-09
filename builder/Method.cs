using script.Container;
using script.stack;

namespace script.builder
{
    public class Method
    {
        private MethodContainer Container;

        public Method(string name)
        {
            Container = Create(name);
        }

        public MethodContainer GetMethodContainer()
        {
            return Container;
        }

        public void SetStatic()
        {
            Container.IsStatic = true;
        }

        public void setPrivate()
        {
            Container.IsPublic = false;
        }

        public void SetAgumentStack(AgumentStack stack)
        {
            Container.Agument = stack;
        }

        public AgumentStack GetAgumentStack()
        {
            return Container.Agument;
        }

        public void SetBody(MethodContainer.MethodDelegate body)
        {
            Container.Body += body;
        }

        public void SetVariabel()
        {
            Container.SetVariabel = true;
        }

        public void SetReturnType(string type)
        {
            Container.ReturnType = type;
        }

        private MethodContainer Create(string name)
        {
            return new MethodContainer()
            {
                Name = name,
                IsStatic = false,
                IsPublic = true,
                Agument = new AgumentStack(),
                SetVariabel = false,
            };
        }
    }
}
