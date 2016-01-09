using script.builder;
using script.help;
using script.stack;
namespace script.variabel
{
    class MethodVariabel : FunctionVariabel
    {
        private ClassMethods method;
        private ObjectVariabel obj;

        public MethodVariabel(ClassMethods method, ObjectVariabel obj) : base(null)
        {
            this.method = method;
            this.obj = obj;
        }

        public override AgumentStack getStatck()
        {
            return method.Aguments;
        }

        public override CVar call(CallAgumentStack call, EnegyData data)
        {
            return method.call(obj, data.VariabelDatabase, call, data);
        }

        public override CVar call(EnegyData data, params object[] parameters)
        {
            CallAgumentStack stack = new CallAgumentStack();
            VariabelDatabase vd = data.VariabelDatabase.createShadow();

            for (int i = 0; i < parameters.Length && i <= method.Aguments.size(); i++)
            {
                CVar context = ScriptConverter.convert(parameters[i]);
                if (method.Aguments.get(i).hasType() && !CallScriptFunction.compare(context, method.Aguments.get(i).Type))
                    throw new ScriptError("Cant convert " + context.type() + " to " + method.Aguments.get(i).Type.ToString(), new Posision(0, 0));

                //okay let cache the parameters :)
                stack.push(context);
                vd.push(method.Aguments.get(i).Name, context);
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for (int i = stack.size(); i < method.Aguments.size(); i++)
            {
                if (!method.Aguments.get(i).hasValue())
                    throw new ScriptError("Missing agument to " + method.Name, new Posision(0, 0));

                stack.push(method.Aguments.get(i).Value);
                vd.push(method.Aguments.get(i).Name, method.Aguments.get(i).Value);
            }

            return call(stack, new EnegyData(vd, new Interprenter(), data.Plugin, data.Error, data));
        }

        public override bool compare(CVar var, Posision pos)
        {
            return false;
        }

        public override string type()
        {
            return "method";
        }
    }
}
