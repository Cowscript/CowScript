using script.builder;
using script.Container;

namespace script.variabel
{
    public class ClassVariabel : CVar
    {
        public ClassContainer Container;

        public virtual string Name { get { return Container.Name; } }

        public ClassVariabel(Class c)
        {
            //here wee get the data about this class :)
            Container = c.GetContainer();
        }

        public bool hasConstructor()
        {
            return Container.Constructor != null;
        }

        public bool ContainItem(string name)
        {
            return Container.StaticMethod.ContainsKey(name) || Container.StaticPointer.ContainsKey(name);
        }

        public bool IsMethod(string name)
        {
            return Container.StaticMethod.ContainsKey(name);
        }

        public bool IsPointer(string name)
        {
            return Container.StaticPointer.ContainsKey(name);
        }

        public bool IsPublic(string name)
        {
            if (IsMethod(name))
                return Container.StaticMethod[name].IsPublic;

            return Container.StaticPointer[name].IsPublic;
        }
        
        public CVar get(string name)
        {
            if (IsMethod(name))
                return new StaticMethodVariabel(Container.StaticMethod[name], this, Container.ExtraVariabelDatabase);

            return new PointerVariabel(Container.StaticPointer[name]);
        }

        public MethodContainer getConstructor()
        {
            return Container.Constructor;
        }

        public ObjectVariabel createNew(VariabelDatabase db, EnegyData data, CVar[] call, Posision pos)
        {
            //wee create a new object and return the object to the system :)
            ObjectVariabel obj = new ObjectVariabel(this, Container.Pointer, Container.Methods, Container.ExtraVariabelDatabase, Container.Extends);

            if (hasConstructor())
            {
                VariabelDatabase vd = db.createShadow(obj);
                if (Container.Constructor.SetVariabel)
                {
                    for (int i = 0; i < Container.Constructor.Agument.size(); i++)
                        vd.push(Container.Constructor.Agument.get(i).Name, call[i], data);
                }
                CallConstructor(vd, call, data, pos, obj);
            }

            return obj;
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (!Container.StaticMethod.ContainsKey("toString"))
                return base.toString(pos, data, db);

            //it contain a method toString
            if (!Container.StaticMethod["toString"].IsPublic)
                return base.toString(pos, data, db);

            //it is public
            if (Container.StaticMethod["toString"].Agument.size() != 0)
                return base.toString(pos, data, db);

            //it has no aguments :)
            StaticMethodVariabel toString = new StaticMethodVariabel(Container.StaticMethod["toString"], this, Container.ExtraVariabelDatabase);
            return toString.call(new CVar[0], toString.getShadowVariabelDatabase(db), data, pos).toString(pos, data, db);
        }

        public override double toInt(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (!Container.StaticMethod.ContainsKey("toInt"))
                return base.toInt(pos, data, db);

            //it contain a method toString
            if (!Container.StaticMethod["toInt"].IsPublic)
                return base.toInt(pos, data, db);

            //it is public
            if (Container.StaticMethod["toInt"].Agument.size() != 0)
                return base.toInt(pos, data, db);

            //it has no aguments :)
            StaticMethodVariabel toInt = new StaticMethodVariabel(Container.StaticMethod["toInt"], this, Container.ExtraVariabelDatabase);
            return toInt.call(new CVar[0], toInt.getShadowVariabelDatabase(db), data, pos).toInt(pos, data, db);
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return false;
        }

        public override string type()
        {
            return "class";
        }

        private void CallConstructor(VariabelDatabase db, CVar[] call, EnegyData data, Posision pos, ObjectVariabel obj)
        {
            new MethodVariabel(Container.Constructor, obj, Container.ExtraVariabelDatabase).call(call, db, data, pos);
        }
    }
}
