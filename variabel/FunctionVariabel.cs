using script.builder;
using script.stack;
using script.Type;
using System;

namespace script.variabel
{
    public class FunctionVariabel : CVar
    {

        public Function func;

        public FunctionVariabel(Function func)
        {
            this.func = func;
        }

        public virtual bool SetVariabel { get { return func.setVariabel; } }

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
            if(func.extraVariabelDatabase != null)
            {
                db = func.extraVariabelDatabase.createShadow();
                if (SetVariabel)
                {
                    for (int i = 0; i < func.agument.size(); i++)
                        db.push(func.agument.get(i).Name, call[i], data);
                }
            }

            CVar r = func.callFunction(call, db, data, pos);
            db.garbageCollector();
            if(func.ReturnType != null)
            {
                //this function is lock to a type :)
                if (r == null)
                    r = new NullVariabel();

                if(!TypeHandler.controlType(r, func.ReturnType))
                {
                    data.setError(new ScriptError("a function '"+func.Name+"' returns can not be convertet to '" + func.ReturnType + "'", pos), db);
                    return new NullVariabel();
                }
            }

            if (r == null)
                return new NullVariabel();

            return r;
        }

        public virtual CVar call(EnegyData data, VariabelDatabase db, params object[] parameters)
        {
            //wee push the parameters in vd :)
            CVar[] stack = new CVar[func.agument.size()];
            AgumentStack agument = getStatck();
            VariabelDatabase vd = getShadowVariabelDatabase(db);
            
            int i = 0;

            for (; i < parameters.Length; i++)
            {
                CVar context = ScriptConverter.convert(parameters[i], data, db);
                if(agument.get(i).hasType() && !TypeHandler.controlType(context, agument.get(i).Type))
                {
                    data.setError(new ScriptError("Cant convert " + context.type() + " to " + agument.get(i).Type.ToString(), new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                stack[i] = context;
            
                if (SetVariabel)
                    vd.push(agument.get(i).Name, context, data);
            }

            for (; i < stack.Length; i++)
            {
                if (!agument.get(i).hasType())
                {
                    data.setError(new ScriptError("Missing agument to " + func.Name, new Posision(0, 0)), db);
                    return new NullVariabel();
                }

                stack[i] = agument.get(i).Value;
                if (SetVariabel)
                    vd.push(agument.get(i).Name, stack[i], data);
            }

            return call(stack, vd, data, new Posision(0, 0));
        }

        public override string type()
        {
            return "function";
        }
    }
}
