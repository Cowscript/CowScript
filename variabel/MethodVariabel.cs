using script.builder;
using script.help;
using script.stack;
namespace script.variabel
{
    class MethodVariabel : FunctionVariabel
    {
        private ClassMethods method;
        private ObjectVariabel obj;
        private VariabelDatabase extraVariabelDatabase;

        public MethodVariabel(ClassMethods method, ObjectVariabel obj, VariabelDatabase extra) : base(null)
        {
            this.method = method;
            this.obj = obj;
            extraVariabelDatabase = extra;
        }

        public override int agumentSize()
        {
            return method.Aguments.size();
        }

        public override AgumentStack getStatck()
        {
            return method.Aguments;
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
                for (int i = 0; i < method.Aguments.size(); i++)
                    db.push(method.Aguments.get(i).Name, call[i], data);
            }
            if(method.ReturnType != null)
            {
                CVar cache = method.call(obj, db, call, data, pos);
                if(!CallScriptFunction.compare(cache, method.ReturnType))
                {
                    data.setError(new ScriptError("a method '" + obj.Name + "->"+method.Name+"' returns can not be convertet to '" + method.ReturnType + "'", pos), db);
                    return new NullVariabel();
                }
            }
            return method.call(obj, db, call, data, pos);
        }

        public override CVar call(EnegyData data, VariabelDatabase db, params object[] parameters)
        {
            CVar[] stack = new CVar[method.Aguments.size()];
            VariabelDatabase vd = getShadowVariabelDatabase(db);

            for (int i = 0; i < parameters.Length && i <= method.Aguments.size(); i++)
            {
                CVar context = ScriptConverter.convert(parameters[i], data, db);
                if (method.Aguments.get(i).hasType() && !CallScriptFunction.compare(context, method.Aguments.get(i).Type))
                {
                    data.setError(new ScriptError("Cant convert " + context.type() + " to " + method.Aguments.get(i).Type.ToString(), new Posision(0, 0)), db);
                }

                //okay let cache the parameters :)
                stack[0] = context;
                vd.push(method.Aguments.get(i).Name, stack[0], data);
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for (int i = stack.Length; i < method.Aguments.size(); i++)
            {
                if (!method.Aguments.get(i).hasValue())
                {
                    data.setError(new ScriptError("Missing agument to " + method.Name, new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                stack[i] = method.Aguments.get(i).Value;
                vd.push(method.Aguments.get(i).Name, method.Aguments.get(i).Value, data);
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
