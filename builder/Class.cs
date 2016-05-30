using script.Container;
using script.variabel;
using System.Collections.Generic;

namespace script.builder
{
    public class Class
    {
        private ClassContainer container;

        public Class(string name)
        {
            container = Create(name);
        }

        public Class()
        {
            container = Create(null);
        }

        public void Extends(ClassVariabel extends)
        {
            container.Extends.Add(extends);
        }

        public ClassContainer GetContainer()
        {
            return container;
        }

        public void SetMethod(Method method, EnegyData data, VariabelDatabase db, Posision pos)
        {
            MethodContainer mc = method.GetMethodContainer();

            if(!Control(mc.Name, data, db, pos))
            {
                return;
            }

            if (mc.IsStatic)
                container.StaticMethod.Add(mc.Name, mc);
            else
                container.Methods.Add(mc.Name, mc);
        }

        public void SetPointer(Pointer pointer, EnegyData data, VariabelDatabase db, Posision pos)
        {
            PointerContainer pc = pointer.GetPointerContainer();

            Control(pc.Name, data, db, pos);//control if the class contains the name already

            if (pc.IsStatic)
                container.StaticPointer.Add(pc.Name, pc);
            else
                container.Pointer.Add(pc.Name, pc);
        }

        public void SetConstructor(Method method, EnegyData data, VariabelDatabase db, Posision pos)
        {
            if(container.Constructor != null)
            {
                data.setError(new ScriptError("A class can only have one constructor", pos), db);
                return;
            }

            container.Constructor = method.GetMethodContainer();
        }
        
        public void SetExtraVariabelDatabase(VariabelDatabase ex)
        {
            container.ExtraVariabelDatabase = ex;
        }

        private ClassContainer Create(string name)
        {
            return new ClassContainer()
            {
                Name = name,
                Methods = new Dictionary<string, MethodContainer>(),
                StaticMethod = new Dictionary<string, MethodContainer>(),
                Pointer = new Dictionary<string, PointerContainer>(),
                StaticPointer = new Dictionary<string, PointerContainer>(),
                Extends = new System.Collections.ArrayList(),
            };
        }

        private bool Control(string name, EnegyData data, VariabelDatabase db, Posision pos)
        {
            if (container.StaticPointer.ContainsKey(name) || container.Pointer.ContainsKey(name) || container.Methods.ContainsKey(name)  || container.StaticMethod.ContainsKey(name))
            {
                data.setError(new ScriptError("You can not put more end one item in the class with the same name", pos), db);
                return false;
            }

            return true;
        }
    }

    public enum ClassItemAccessLevel
    {
        Public, 
        Private,
        Protected,
    }
}
