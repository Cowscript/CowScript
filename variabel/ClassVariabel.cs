using script.builder;
using script.help;
using script.stack;
using System.Collections.Generic;

namespace script.variabel
{
    public class ClassVariabel : CVar
    {
        private Class Container;
        protected Dictionary<string, ClassStaticData> staticItems = new Dictionary<string, ClassStaticData>();

        public virtual string Name { get { return Container.Name; } }

        public ClassVariabel(Class c)
        {
            Container = c;
            c.setStaticData(staticItems, this);
        }

        public ClassStaticData getItem(string name)
        {
            return staticItems[name];
        }

        public bool containsStaticItem(string name)
        {
            return staticItems.ContainsKey(name);
        }

        public virtual bool hasConstructor()
        {
            return Container.constructor != null;
        }

        public virtual ClassMethods getConstructor()
        {
            return Container.constructor;
        }

        public virtual void callConstructor(CallAgumentStack c, VariabelDatabase db, EnegyData d, ObjectVariabel obj)
        {
            if (!hasConstructor())
                return;

            new MethodVariabel(Container.constructor, obj).call(c, d);
        }

        public override bool compare(CVar var, Posision pos)
        {
            return false;
        }

        public override string type()
        {
            return "class";
        }

        public ObjectVariabel createNew(CallAgumentStack cas, VariabelDatabase vd, EnegyData data)
        {
            ObjectVariabel obj = Container.createObject();

            if (hasConstructor())
                callConstructor(cas, vd, data, obj);

            return obj;    
        }
    }
}
