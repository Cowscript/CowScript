using script.variabel;
using script.token;
using script.plugin;
using System.Collections;
using script.stack;
using script.parser;
using System;
using script.Type;

namespace script.help
{
    class CallScriptFunction
    {
        private ArrayList body;
        private PluginContainer plugin;

        public CallScriptFunction(ArrayList body, PluginContainer plugin)
        {
            this.body = body;
            this.plugin = plugin;
        }

        public CVar call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            Interprenter.parse(new TokenCache(body, data, db), data, db);

            if (data.State == RunningState.Return)
                return data.getReturn();

            return new NullVariabel();
        }
        
        public static ParseFunctionCallResult parseCall(bool parse, AgumentStack stack, VariabelDatabase vd, Token token, VariabelParser parser, EnegyData data, bool setVariabel)
        {
            CVar[] call = new CVar[EnegyData.getAgumentSize(data)];

            if (token.next().type() != TokenType.RightBue)
            {
                CVar v = parser.getAsk(parse);
                if (stack.size() > 0)
                {
                    if (stack.get(0).hasType())
                    {
                        if (!TypeHandler.controlType(v, stack.get(0).Type))
                        {
                            data.setError(new ScriptError("A agument number 0 in function can not be convertet from '" + v.type() + "' to " + stack.get(0).Type.ToString(), token.getCache().posision()), vd);
                            return null;
                        }

                        v = getTrueType(v, stack.get(0).Type, data, vd, token.getCache().posision());
                    }

                    call[0] = v;
                    if(setVariabel)
                        vd.push(stack.get(0).Name, v, data);
                }

                int i = 1;
                while (token.getCache().type() == TokenType.Comma)
                {
                    token.next();
                    v = parser.getAsk(parse);
                    if (stack.size() >= i)
                    {
                        if (stack.get(i).hasType())
                        {
                            if (!TypeHandler.controlType(v, stack.get(i).Type))
                            {
                                data.setError(new ScriptError("A agument number " + (i + 1) + " in function can not be convertet to " + stack.get(i).Type.ToString(), token.getCache().posision()), vd);
                                return null;
                            }

                            v = getTrueType(v, stack.get(i).Type, data, vd, token.getCache().posision());
                        }
                        
                        call[i] = v;
                        if(setVariabel)
                            vd.push(stack.get(i).Name, v, data);
                    }

                    i++;
                }

                //control if wee missing aguments :)
                for (int a = i; a < stack.size(); a++)
                {
                    if (!stack.get(a).hasValue())
                    {
                        data.setError(new ScriptError("Call of function missing aguments", token.getCache().posision()), vd);
                        return null;
                    }

                    call[a] = stack.get(a).Value;
                    if(setVariabel)
                        vd.push(stack.get(a).Name, stack.get(a).Value, data);
                }
            }
            else
            {
                //okay wee need to control if it taks 0 aguments :)
                if (stack.size() != 0)
                {
                    //it taks aguments :) let see if one ore more requerie aguments :)
                    for (int i = 0; i < stack.size(); i++)
                    {
                        if (!stack.get(i).hasValue())
                        {
                            data.setError(new ScriptError("Call of function missing aguments", token.getCache().posision()), vd);
                            return null;
                        }

                        if(setVariabel)
                            vd.push(stack.get(i).Name, stack.get(i).Value, data);

                        call[i] = stack.get(i).Value;
                    }
                }
            }

            if (token.getCache().type() != TokenType.RightBue)
            {
                data.setError(new ScriptError("Missing ) got " + token.getCache().ToString(), token.getCache().posision()), vd);
                return null;
            }

            token.next();

            return new ParseFunctionCallResult()
            {
                VariabelDatabase = vd,
                Call = call
            };
            
        }

        private static CVar getTrueType(CVar var, string type, EnegyData data, VariabelDatabase db, Posision pos)
        {
            //this is for int and string if it not this two proces 
            switch (type)
            {
                case "string":
                    if (StringVariabel.isString(var))
                        return var;

                    return StringVariabel.CreateString(data, db, pos, var.toString(pos, data, db));
                case "int":
                    if (IntVariabel.isInt(var))
                        return var;
                    return IntVariabel.createInt(data, db, pos, var.toInt(pos, data, db));
                default:
                    return var;
            }
        }
    }

    class ParseFunctionCallResult
    {
        public VariabelDatabase VariabelDatabase { set; get; }
        public CVar[] Call { set; get; }
    }
}
