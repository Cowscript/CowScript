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
        private VariabelDatabase extra = null;

        public virtual string Name { get { return Container.Name; } }

        public ClassVariabel(Class c)
        {
            Container = c;
            extra = c.extraVariabelDatabase;
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

        public virtual void callConstructor(CVar[] c, VariabelDatabase db, EnegyData d, ObjectVariabel obj, Posision pos)
        {
            if (!hasConstructor())
                return;

            new MethodVariabel(Container.constructor, obj, extra).call(c, db, d, pos);
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return false;
        }

        public override string type()
        {
            return "class";
        }

        public override string toString(Posision pos, EnegyData data, VariabelDatabase db)
        {
            if (!containsStaticItem("toString"))
                return base.toString(pos, data, db);

            ClassStaticData d = getItem("toString");

            if (!d.isPublic)
                return base.toString(pos, data, db);

            if (!d.isMethod)
                return base.toString(pos, data, db);

            if (((StaticMethodVariabel)d.Context).agumentSize() != 0)
                return base.toString(pos, data, db);

            return ((MethodVariabel)d.Context).call(new CVar[0], db, data, pos).toString(pos, data, db);
        }

        public ObjectVariabel createNew(VariabelDatabase db, EnegyData data, Posision pos, params object[] inputs)
        {
            int length = hasConstructor() ? Container.constructor.Aguments.size() : 0;
            AgumentStack a = hasConstructor() ? Container.constructor.Aguments : new AgumentStack();

            CVar[] stack = new CVar[length];
            int i = 0;
            for (; i < length && i < inputs.Length; i++)
            {
                CVar context = ScriptConverter.convert(inputs[i], data, db);
                if (a.get(i).hasType() && !CallScriptFunction.compare(context, a.get(i).Type))
                {
                    data.setError(new ScriptError("Cant convert " + context.type() + " to " + a.get(i).Type.ToString(), new Posision(0, 0)), db);
                    return null;
                }

                //okay let cache the parameters :)
                stack[i] = context;
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for (; i < a.size(); i++)
            {
                if (!a.get(i).hasValue())
                {
                    data.setError(new ScriptError("Missing agument", new Posision(0, 0)), db);
                    return null;
                }

                stack[i] = a.get(i).Value;
            }

            return createNew(stack, db, data, pos);
        }

        public ObjectVariabel createNew(CVar[] cas, VariabelDatabase db, EnegyData data, Posision pos)
        {
            ObjectVariabel obj = Container.createObject();

            if (hasConstructor())
            {
                VariabelDatabase vd = db.createShadow(obj);
                //wee quikly push variabel in a shadow of our database :)
                for (int i = 0; i < getConstructor().Aguments.size(); i++)
                    vd.push(getConstructor().Aguments.get(i).Name, cas[i], data);

                callConstructor(cas, vd, data, obj, pos);
            }

            return obj;    
        }
    }
}
