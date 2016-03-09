using script.Container;
using script.Type;
using System.Collections;
using System.Collections.Generic;

namespace script.variabel
{
    public class ObjectVariabel : CVar
    {
        public Dictionary<string, object> systemItems = new Dictionary<string, object>();
        public ClassItemContainer[] Items;
        private ClassVariabel owner;

        public string Name { get {
                return owner.Name;
            }
        }

        public ArrayList GetExtends
        {
            get
            {
                return owner.Container.Extends;
            }
        }

        public ObjectVariabel(ClassVariabel c, Dictionary<string, PointerContainer> pointer, Dictionary<string, MethodContainer> Method, VariabelDatabase extra, ArrayList Extends)
        {
            owner = c;
            Items = new ClassItemContainer[pointer.Count + Method.Count];
            int i = 0;
            
            foreach(PointerContainer p in pointer.Values)
            {
                Items[i] = new ClassItemContainer()
                {
                    Name = p.Name,
                    Context = p.DefaultValue,
                    IsPointer = true,
                    IsPublic = p.IsPublic
                };
                i++;
            }

            foreach(MethodContainer m in Method.Values)
            {
                Items[i] = new ClassItemContainer()
                {
                    Name = m.Name,
                    Context = new MethodVariabel(m, this, extra),
                    IsPointer = false,
                    IsPublic = m.IsPublic
                };
                i++;
            }

            AppendExtends(extra);
        }

        public CVar get(string name)
        {
            int i;
            if ((i = getIDByName(name)) != -1)
            {
                return Items[i].Context;
            }

            return null;
        }

        public bool isPointer(string name)
        {
            int i;
            if((i = getIDByName(name)) != -1)
            {
                return Items[i].IsPointer;
            }

            return false;
        }

        public bool isPublic(string name)
        {
            int i;
            if((i = getIDByName(name)) != -1)
            {
                return Items[i].IsPublic;
            }

            return false;
        }

        public void appendToPointer(string name, CVar value)
        {
            int i;
            if((i=getIDByName(name)) != -1)
            {
                if (Items[i].IsPointer)
                {
                    Items[i].Context = value;
                }
            }
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (Types.instanceof((ClassVariabel)db.get("string", data), this))
            {
                return toString(pos, data, db) == var.toString(pos, data, db);
            }else if (Types.instanceof((ClassVariabel)db.get("int", data), this))
            {
                return toInt(pos, data, db) == var.toInt(pos, data, db);
            }

            return this == var;
        }

        public override string type()
        {
            return "object";
        }

        public bool containsItem(string name)
        {
            return getIDByName(name) != -1;
        }

        public override double toInt(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (Name == "int")
            {
                return (double)systemItems["int"];
            }

            MethodContainer t;
            MethodVariabel toInt;

            //wee control this class has a method 'toString'
            if (owner.Container.Methods.ContainsKey("toInt"))
            {
                t = owner.Container.Methods["toInt"];
                //let us test it and see what happens :)
                if (t.Agument.size() == 0 && t.IsPublic)
                {
                    toInt = (MethodVariabel)Items[getIDByName("toInt")].Context;
                    return toInt.call(new CVar[0], toInt.getShadowVariabelDatabase(db), data, pos).toInt(pos, data, db);
                }
            }

            bool extendsInt = false;//fall back if not method has giving and it extends string

            //okay this class has no method "toString" now wee look in the other extends method.
            foreach (ClassVariabel extends in GetExtends)
            {
                //look it in and see if this is 1: not string. 2 has a toString method width no agument and is public.
                if (extends.Name != "int" && extends.Container.Methods.ContainsKey("toInt"))
                {
                    t = extends.Container.Methods["toInt"];
                    if (t.Agument.size() == 0 && t.IsPublic)
                    {
                        toInt = (MethodVariabel)Items[getIDByName("toInt")].Context;
                        return toInt.call(new CVar[0], toInt.getShadowVariabelDatabase(db), data, pos).toInt(pos, data, db);
                    }
                }
                else if (extends.Name == "int")
                {
                    extendsInt = true;
                }
            }

            if (extendsInt)
            {
                return 0;
            }

            return base.toInt(pos, data, db);
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if(Name == "string")
            {
                return (string)systemItems["str"];
            }

            MethodContainer t;
            MethodVariabel toString;

            //wee control this class has a method 'toString'
            if (owner.Container.Methods.ContainsKey("toString"))
            {
                t = owner.Container.Methods["toString"];
                //let us test it and see what happens :)
                if(t.Agument.size() == 0 && t.IsPublic)
                {
                    toString = (MethodVariabel)Items[getIDByName("toString")].Context;
                    return toString.call(new CVar[0], toString.getShadowVariabelDatabase(db), data, pos).toString(pos, data, db);
                }
            }

            bool extendsString = false;//fall back if not method has giving and it extends string

            //okay this class has no method "toString" now wee look in the other extends method.
            foreach(ClassVariabel extends in GetExtends)
            {
                //look it in and see if this is 1: not string. 2 has a toString method width no agument and is public.
                if(extends.Name != "string" && extends.Container.Methods.ContainsKey("toString"))
                {
                    t = extends.Container.Methods["toString"];
                    if(t.Agument.size() == 0 && t.IsPublic)
                    {
                        toString = (MethodVariabel)Items[getIDByName("toString")].Context;
                        return toString.call(new CVar[0], toString.getShadowVariabelDatabase(db), data, pos).toString(pos, data, db);
                    }
                }else if(extends.Name == "string")
                {
                    extendsString = true;
                }
            }

            if (extendsString)
            {
                return "";
            }

            return base.toString(pos, data, db);
        }

        private void AppendExtends(VariabelDatabase extra)
        {
            foreach(ClassVariabel c in GetExtends)
            {
                foreach(PointerContainer p in c.Container.Pointer.Values)
                {
                    if(!p.IsStatic && !containsItem(p.Name))
                    {
                        Resize(new ClassItemContainer()
                        {
                            Name = p.Name,
                            Context = p.DefaultValue,
                            IsPointer = true,
                            IsPublic = p.IsPublic
                        });
                    }
                }

                foreach(MethodContainer m in c.Container.Methods.Values)
                {
                    if(!m.IsStatic && !containsItem(m.Name))
                    {
                        Resize(new ClassItemContainer()
                        {
                            Name = m.Name,
                            Context = new MethodVariabel(m, this, extra),
                            IsPointer = false,
                            IsPublic = m.IsPublic
                        });
                    }
                }
            }
        }

        private void Resize(ClassItemContainer item)
        {
            ClassItemContainer[] cache = new ClassItemContainer[Items.Length + 1];

            for(int i = 0; i < Items.Length; i++)
            {
                cache[i] = Items[i];
            }

            cache[Items.Length] = item;
            Items = cache;
        }

        private int getIDByName(string name)
        {
            for(int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Name == name)
                    return i;
            }

            return -1;
        }
    }
}
