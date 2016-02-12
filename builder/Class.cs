using script.help;
using script.stack;
using script.variabel;
using System.Collections;
using System.Collections.Generic;

namespace script.builder
{
    public class Class
    {
        private Dictionary<string, ClassItems> items = new Dictionary<string, ClassItems>();
        public string Name { private set; get; }
        public ClassMethods constructor { private set; get; }
        private EnegyData data;
        private VariabelDatabase db;
        public VariabelDatabase extraVariabelDatabase { get; set; }

        public Class(string name, EnegyData data, VariabelDatabase db) { Name = name; this.data = data; this.db = db; }
        public Class(string name) { Name = name; data = new EnegyData(); }
        public Class() {data = new EnegyData(); }

        public void setStaticData(Dictionary<string, ClassStaticData> i, ClassVariabel var)
        {
            foreach(ClassItems d in items.Values)
            {
                if (d.IsMethod && d.Method.isStatic)
                    i.Add(d.Name, new ClassStaticData()
                    {
                        isMethod = true,
                        Context = new StaticMethodVariabel((ClassStaticMethods)d.Method, var, extraVariabelDatabase),
                        isPublic = d.Method.isPublic,
                        extraVariabelDatabase = extraVariabelDatabase,
                    });
                else if (!d.IsMethod && d.IsStatic)
                    i.Add(d.Name, new ClassStaticData() {
                        isMethod = false,
                        Context = d.Context,
                        isPublic = d.IsPublic,
                        extraVariabelDatabase = extraVariabelDatabase,
                    });
            }
        }

        public void addVariabel(string name, CVar context, bool isStatic, bool isPublic)
        {
            if (!controlItems(name))
                return;

            items.Add(name, new ClassItems()
            {
                IsMethod = false,
                Name = name,
                Context = context,
                IsStatic = isStatic,
                IsPublic = isPublic,
                extraVariabelDatabase = extraVariabelDatabase,
            });
        }

        public void addVariabel(string name, bool isStatic, bool isPublic)
        {
            addVariabel(name, new NullVariabel(), isStatic, isPublic);
        }

        public void addMethod(ClassItemsMethod m)
        {
            if (!controlItems(m.Name))
            {
                data.setError(new ScriptError("You can only add one method: " + m.Name, new Posision(0, 0)), db);
                return;
            }

            items.Add(m.Name, new ClassItems()
            {
                IsMethod = true,
                Method = m,
                Name = m.Name,
                extraVariabelDatabase = extraVariabelDatabase,
            });
        }

        public void addConstructor(ClassMethods m)
        {
            if (constructor != null)
            {
                data.setError(new ScriptError("A class can only have one constructor", new Posision(0, 0)), db);
                return;
            }

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
                        Context = new MethodVariabel((ClassMethods)item.Method, obj, extraVariabelDatabase),
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
                        isStatic = item.IsStatic,
                        extraVariabelDatabase = extraVariabelDatabase,
                    });
            }

            return obj;
        }

        private bool controlItems(string name)
        {
            if (items.ContainsKey(name)) {
                data.setError(new ScriptError("A class items i alredy exists: " + name, new Posision(0, 0)), db);
                return false;
            }

            return true;
        }
    }

    public abstract class ClassItemsMethod
    {
        public string Name { protected set; get; }
        public bool isPublic { protected set; get; }
        public bool isStatic { protected set; get; }
        public AgumentStack Aguments {set; get; }
        public string ReturnType { protected set; get; }
        protected Class c;

        public ClassItemsMethod(Class c, string name, string type)
        {
            Name = name;
            isPublic = true;
            isStatic = false;
            Aguments = new AgumentStack();
            ReturnType = type;
            this.c = c;
        }

        public ClassItemsMethod(Class c, string name, bool isPublic, string type)
        {
            Name = name;
            this.isPublic = isPublic;
            isStatic = false;
            Aguments = new AgumentStack();
            ReturnType = type;
            this.c = c;
        }

        public ClassItemsMethod(Class c, string name, bool isPublic, bool isStatic, string type)
        {
            Name = name;
            this.isPublic = isPublic;
            this.isStatic = isStatic;
            Aguments = new AgumentStack();
            ReturnType = type;
            this.c = c;
        }

        public void create()
        {
            c.addMethod(this);
        }
    }

    public class ClassStaticMethods : ClassItemsMethod
    {
        public delegate CVar Method(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos);
        public event Method caller;

        public ClassStaticMethods(Class c, string name) : base(c, name, true, true, null)
        {
        }

        public ClassStaticMethods(Class c, string name, bool isPublic, string type) : base(c, name, isPublic, true, type)
        {
        }

        public CVar call(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            CVar cc = caller(c, db, stack, data, pos);

            if (cc == null)
                return new NullVariabel();

            return cc;
        }
    }

    public class ClassMethods : ClassItemsMethod
    {
        public delegate CVar Method(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos);
        public event Method caller;

        public ClassMethods(Class c, string name) : base(c, name, null)
        {
        }

        public ClassMethods(Class c, string name, bool isPublic, string type) : base(c, name, isPublic, type)
        {
        }

        public void createConstructor()
        {
            c.addConstructor(this);
        }

        public CVar call(ObjectVariabel obj, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            CVar c = caller(obj, db, stack, data, pos);

            if (c == null)
                return new NullVariabel();

            return c;
        }
    }
}
