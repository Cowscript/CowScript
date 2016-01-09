using script.help;
using script.stack;
using script.variabel;
using System.Collections.Generic;

namespace script.builder
{
    public class Class
    {
        private Dictionary<string, ClassItems> items = new Dictionary<string, ClassItems>();
        public string Name { private set; get; }
        public ClassMethods constructor { private set; get; }

        public Class(string name) { Name = name; }

        public void setStaticData(Dictionary<string, ClassStaticData> i, ClassVariabel var)
        {
            foreach(ClassItems d in items.Values)
            {
                if (d.Name == null)
                    throw new ScriptError("d.Name must not be null", new Posision(0, 0));

                if (d.IsMethod && d.Method.isStatic)
                    i.Add(d.Name, new ClassStaticData()
                    {
                        isMethod = true,
                        Context = new StaticMethodVariabel((ClassStaticMethods)d.Method, var),
                        isPublic = d.Method.isPublic
                    });
                else if (!d.IsMethod && d.IsStatic)
                    i.Add(d.Name, new ClassStaticData() {
                        isMethod = false,
                        Context  = d.Context,
                        isPublic = d.IsPublic
                    });
            }
        }

        public ClassItemsMethod createMethods()
        {
            return createMethods(false);
        }

        public ClassItemsMethod createMethods(bool isStatic)
        {
            if (isStatic)
                return new ClassStaticMethods(this);

            return new ClassMethods(this);
        }

        public void addVariabel(string name, CVar context, bool isStatic, bool isPublic)
        {
            controlItems(name);

            items.Add(name, new ClassItems()
            {
                IsMethod = false,
                Name = name,
                Context = context,
                IsStatic = isStatic,
                IsPublic = isPublic
            });
        }

        public void addVariabel(string name, bool isStatic, bool isPublic)
        {
            addVariabel(name, new NullVariabel(), isStatic, isPublic);
        }

        public void addMethod(ClassItemsMethod m)
        {
            controlItems(m.Name);

            items.Add(m.Name, new ClassItems()
            {
                IsMethod = true,
                Method   = m,
                Name = m.Name,
            });
        }

        public void addConstructor(ClassMethods m)
        {
            if (constructor != null)
                throw new ScriptError("A class can only have one constructor", new Posision(0, 0));

            constructor = m;
        }

        public ObjectVariabel createObject()
        {
            ObjectVariabel obj = new ObjectVariabel(Name);

            foreach(ClassItems item in items.Values)
            {
                if (item.IsMethod)
                {
                    if (item.Method.isStatic)
                        continue;
                    obj.put(new ObjectItemData()
                    {
                        isMethod = true,
                        Name = item.Method.Name,
                        Context = new MethodVariabel((ClassMethods)item.Method, obj),
                        isPublic = item.Method.isPublic,
                        isStatic = item.Method.isStatic
                    });
                }
                else
                    obj.put(new ObjectItemData()
                    {
                        isMethod = false,
                        Name = item.Name,
                        Context = item.Context,
                        isPublic = item.IsPublic,
                        isStatic = item.IsStatic
                    });
            }

            return obj;
        }

        private void controlItems(string name)
        {
            if (items.ContainsKey(name))
                throw new ScriptError("A class items i alredy exists: " + name, new Posision(0, 0));
        }
    }

    public abstract class ClassItemsMethod
    {
        public string Name { protected set; get; }
        public bool isPublic { protected set; get; }
        public bool isStatic { protected set; get; }
        public AgumentStack Aguments { set; get; }
        protected Class c;

        public void setAccess(bool isPublic)
        {
            this.isPublic = isPublic;
        }

        public void setName(string name)
        {
            Name = name;
        }

        public void create()
        {
            if (Name == null)
                throw new ScriptError("You can`t create a method whitout a name", new Posision(0, 0));

            if (Aguments == null)
                Aguments = new AgumentStack();

            c.addMethod(this);
        }
    }

    public class ClassStaticMethods : ClassItemsMethod
    {
        public delegate CVar Method(ClassVariabel c, VariabelDatabase db, CallAgumentStack stack, EnegyData data);
        public event Method caller;

        public ClassStaticMethods(Class c)
        {
            this.c = c;
            isStatic = true;
        }

        public CVar call(ClassVariabel c, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            CVar cc = caller(c, db, stack, data);

            if (cc == null)
                return new NullVariabel();

            return cc;
        }
    }

    public class ClassMethods : ClassItemsMethod
    {
        public delegate CVar Method(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data);
        public event Method caller;

        public ClassMethods(Class c)
        {
            this.c = c;
            isStatic = false;
        }

        public void createConstructor()
        {
            if (Aguments == null)
                Aguments = new AgumentStack();

            c.addConstructor(this);
        }

        public CVar call(ObjectVariabel obj, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            CVar c = caller(obj, db, stack, data);

            if (c == null)
                return new NullVariabel();

            return c;
        }
    }
}
