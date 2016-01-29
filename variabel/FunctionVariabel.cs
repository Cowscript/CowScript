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

        public override bool compare(CVar var, Posision pos, EnegyData data, VariabelDatabase db)
        {
            return false;
        }

        public virtual AgumentStack getStatck()
        {
            return func.agument;
        }

        public virtual int agumentSize()
        {
            return func.agument.size();
        }

        public virtual VariabelDatabase getShadowVariabelDatabase(VariabelDatabase db)
        {
            return db.createShadow();
        }

        public virtual CVar call(CVar[] call, VariabelDatabase db, EnegyData data, Posision pos)
        {
            CVar r = func.callFunction(call, db, data, pos);

            if (r == null)
                return new NullVariabel();

            return r;
        }

        public virtual CVar call(EnegyData data, VariabelDatabase db, params object[] parameters)
        {
            CVar[] stack = new CVar[func.agument.size()];
            VariabelDatabase vd = db.createShadow();
            int i = 0;
            for(; i < parameters.Length && i <= func.agument.size(); i++)
            {
                CVar context = ScriptConverter.convert(parameters[i], data, db);
                if (func.agument.get(i).hasType() && !CallScriptFunction.compare(context, func.agument.get(i).Type))
                {
                    data.setError(new ScriptError("Cant convert " + context.type() + " to " + func.agument.get(i).Type.ToString(), new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                //okay let cache the parameters :)
                stack[i] = context;
                vd.push(func.agument.get(i).Name, context, data);
            }

            //wee take a new for loop to get other parameters there is not has been set :)
            for(; i < func.agument.size(); i++)
            {
                if (!func.agument.get(i).hasValue())
                {
                    data.setError(new ScriptError("Missing agument to " + func.Name, new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                stack[i] = func.agument.get(i).Value;
                vd.push(func.agument.get(i).Name, stack[i], data);
            }

            return call(stack, db, data, new Posision(0,0));
        }

        public override string type()
        {
            return "function";
        }
    }
}
