
using script.builder;
using script.Container;
using script.stack;
using script.Type;

namespace script.variabel
{
    public class MethodVariabel : FunctionVariabel
    {
        private ObjectVariabel obj;
        private VariabelDatabase extraVariabelDatabase;
        public MethodContainer method;

        public MethodVariabel(MethodContainer method, ObjectVariabel obj, VariabelDatabase extra) : base(null)
        {
            this.method = method;
            this.obj = obj;
            extraVariabelDatabase = extra;
        }

        public override bool SetVariabel
        {
            get
            {
                return method.SetVariabel;
            }
        }

        public ClassItemAccessLevel GetLevel
        {
            get
            {
                return method.Level;
            }
        }

        public override int agumentSize()
        {
            return method.Agument.size();
        }

        public override AgumentStack getStatck()
        {
            return method.Agument;
        }

        public override VariabelDatabase getShadowVariabelDatabase(VariabelDatabase db)
        {
            return db.createShadow(obj);
        }

        public override CVar call(CVar[] call, VariabelDatabase db, EnegyData data, Posision pos)
        {
            if(extraVariabelDatabase != null)
            {
                db = getShadowVariabelDatabase(extraVariabelDatabase);
                if (SetVariabel)
                {
                    for (int i = 0; i < method.Agument.size(); i++)
                        db.push(method.Agument.get(i).Name, call[i], data);
                }
            }

            CVar cache = method.Body(obj, db, call, data, pos);
            db.garbageCollector();
            if(method.ReturnType != null)
            {
                if(!TypeHandler.controlType(cache, method.ReturnType))
                {
                    data.setError(new ScriptError("a method '" + obj.Name + "->"+method.Name+"' returns can not be convertet to '" + method.ReturnType + "'", pos), db);
                    return new NullVariabel();
                }
            }

            return cache;
        }

        public override CVar call(EnegyData data, VariabelDatabase db, params object[] parameters)
        {
            CVar[] stack = new CVar[method.Agument.size()];
            VariabelDatabase vd = getShadowVariabelDatabase(db);

            for (int i = 0; i < parameters.Length && i <= method.Agument.size(); i++)
            {
                CVar context = ScriptConverter.convert(parameters[i], data, db);
                if (method.Agument.get(i).hasType() && !TypeHandler.controlType(context, method.Agument.get(i).Type))
                {
                    data.setError(new ScriptError("Cant convert " + context.type() + " to " + method.Agument.get(i).Type.ToString(), new Posision(0, 0)), db);
                }

                //okay let cache the parameters :)
                stack[0] = context;
                if(SetVariabel)
                    vd.push(method.Agument.get(i).Name, stack[0], data);
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for (int i = stack.Length; i < method.Agument.size(); i++)
            {
                if (!method.Agument.get(i).hasValue())
                {
                    data.setError(new ScriptError("Missing agument to " + method.Name, new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                stack[i] = method.Agument.get(i).Value;
                if(SetVariabel)
                    vd.push(method.Agument.get(i).Name, method.Agument.get(i).Value, data);
            }

            return call(stack, db, data, new Posision(0,0));
        }

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return false;
        }

        public override string type()
        {
            return "method";
        }
    }
}
