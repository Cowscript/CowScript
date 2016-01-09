using script.builder;
using script.help;
using script.stack;

namespace script.variabel
{
    public class FunctionVariabel : CVar
    {

        public Function func;

        public FunctionVariabel(Function func)
        {
            this.func = func;
        }

        public override bool compare(CVar var, Posision pos)
        {
            return false;
        }

        public virtual AgumentStack getStatck()
        {
            return func.agument;
        }

        public virtual CVar call(CallAgumentStack call, EnegyData data)
        {
            CVar r = func.call.call(call, data);

            if (r == null)
                return new NullVariabel();

            return r;
        }

        public virtual CVar call(EnegyData data, params object[] parameters)
        {
            CallAgumentStack stack = new CallAgumentStack();
            VariabelDatabase vd = data.VariabelDatabase.createShadow();
            
            for(int i = 0; i < parameters.Length && i <= func.agument.size(); i++)
            {
                CVar context = ScriptConverter.convert(parameters[i]);
                if (func.agument.get(i).hasType() && !CallScriptFunction.compare(context, func.agument.get(i).Type))
                    throw new ScriptError("Cant convert " + context.type() + " to " + func.agument.get(i).Type.ToString(), new Posision(0, 0));

                //okay let cache the parameters :)
                stack.push(context);
                vd.push(func.agument.get(i).Name, context);
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for(int i = stack.size(); i < func.agument.size(); i++)
            {
                if (!func.agument.get(i).hasValue())
                    throw new ScriptError("Missing agument to " + func.Name, new Posision(0, 0));

                stack.push(func.agument.get(i).Value);
                vd.push(func.agument.get(i).Name, func.agument.get(i).Value);
            }

            return call(stack, new EnegyData(vd, new Interprenter(), data.Plugin, data.Error, data));
        }

        public override string type()
        {
            return "function";
        }
    }
}
