using System;
using script.token;
using script.variabel;
using script.builder;
using script.help;
using script.stack;

namespace script.parser
{
    class VariabelParser : ParserInterface
    {
        private EnegyData data;
        private Token token;

        public void end()
        {
            if (token.getCache().type() != TokenType.End)
                throw new ScriptError("Missing ;. got: "+token.getCache().ToString(), token.getCache().posision());

            token.next();//update token soo next dont need to use Token.next();
        }

        public CVar parse(EnegyData ed, Token token)
        {
            this.token = token;
            data = ed;
            return getBooleanPrefix(true);
        }

        private CVar getBooleanPrefix(bool parse)
        {
            if(token.getCache().type() == TokenType.Not)
            {
                token.next();
                if (parse)
                    return new BooleanVariabel(!getBoolean(true).toBoolean(token.getCache().posision()));
                return getBoolean(false);
            }

            return getBoolean(parse);
        }

        private CVar getBoolean(bool parse)
        {
            CVar var = getBooleanAnd(parse);

            if(token.getCache().type() == TokenType.Or)
            {
                token.next();
                if (!var.toBoolean(token.getCache().posision()))
                {
                    //this is not true soo wee use this :)
                    return new BooleanVariabel(getBooleanPrefix(parse).toBoolean(token.getCache().posision()));
                }

                getBooleanPrefix(false);//no need this becuse it is true :)
                return new BooleanVariabel(true);
            }

            return var;
        }

        private CVar getBooleanAnd(bool parse)
        {
            CVar var = getBooleanFactor(parse);

            if(token.getCache().type() == TokenType.And)
            {
                token.next();
                if (!var.toBoolean(token.getCache().posision()))
                {
                    getBooleanAnd(false);
                    return new BooleanVariabel(false);//wee dont use this :)
                }

                return new BooleanVariabel(getBooleanAnd(parse).toBoolean(token.getCache().posision()));
            }

            return var;
        }

        private CVar getBooleanFactor(bool parse)
        {
            CVar var = booleanRelation(parse);

            if(token.getCache().type() == TokenType.Not)
            {
                token.next();
                return new BooleanVariabel(!booleanRelation(parse).toBoolean(token.getCache().posision()));
            }

            return var;
        }

        private CVar booleanRelation(bool parse)
        {
            CVar var = sum(parse);

            if (token.getCache().type() == TokenType.Equels) //==
            {
                token.next();
                return new BooleanVariabel(sum(parse).compare(var, token.getCache().posision()));
            }
            else if (token.getCache().type() == TokenType.NotEquels) //!=
            {
                token.next();
                return new BooleanVariabel(!sum(parse).compare(var, token.getCache().posision()));
            }
            else if (token.getCache().type() == TokenType.GreaterEquels) // <=
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision()) >= var.toInt(token.getCache().posision()));
            }
            else if (token.getCache().type() == TokenType.Greater)
            { //<
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision()) > var.toInt(token.getCache().posision()));
            }else if(token.getCache().type() == TokenType.LessEquels)
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision()) <= var.toInt(token.getCache().posision()));
            }else if(token.getCache().type() == TokenType.Less)
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision()) < var.toInt(token.getCache().posision()));
            }

            return var;
        }

        private CVar sum(bool parse)
        {
            CVar var = gange(parse);

            TokenBuffer buffer;
            while ((buffer = token.getCache()).type() == TokenType.Plus || buffer.type() == TokenType.Minus)
            {
                token.next();
                if(buffer.type() == TokenType.Plus)
                {
                    CVar b = gange(parse);
                    if (b is IntVariabel && var is IntVariabel)
                        var = new IntVariabel(var.toInt(buffer.posision()) + b.toInt(token.getPosision()));
                    else
                        var = new StringVariabel(var.toString(buffer.posision()) + b.toString(token.getPosision()));
                }
                else
                {
                    var = new IntVariabel(var.toInt(buffer.posision()) - gange(parse).toInt(token.getPosision()));
                }
            }

            return var;
        }

        private CVar gange(bool parse)
        {
            CVar var = negetave(parse);

            TokenBuffer buffer;
            while((buffer = token.getCache()).type() == TokenType.Gange || buffer.type() == TokenType.Divider)
            {
                token.next();
                if(buffer.type() == TokenType.Gange)
                {
                    var = new IntVariabel(var.toInt(buffer.posision()) * gange(parse).toInt(token.getPosision()));
                }
                else
                {
                    var = new IntVariabel(var.toInt(buffer.posision()) / gange(parse).toInt(token.getPosision()));
                }
            }

            return var;
        }

        private CVar negetave(bool parse)
        {
            
            if(token.getCache().type() == TokenType.Minus)
            {
                token.next();
                if (parse)
                    return new IntVariabel(-atom(parse).toInt(token.getCache().posision()));
                return atom(parse);
            }else if(token.getCache().type() == TokenType.Plus)
            {
                token.next();
                if (parse)
                    return new IntVariabel(+atom(parse).toInt(token.getCache().posision()));
                return atom(parse);
            }

            return atom(parse);
        }

        private CVar atom(bool parse)
        {
            TokenBuffer buffer;

            if ((buffer = token.getCache()).type() == TokenType.String)
            {
                token.next();
                return new StringVariabel(buffer.ToString());
            }
            else if (buffer.type() == TokenType.Int)
            {
                token.next();
                return new IntVariabel(Convert.ToDouble(buffer.ToString()));
            }
            else if (buffer.type() == TokenType.Bool)
            {
                token.next();
                return new BooleanVariabel(buffer.ToString() == "true");
            }
            else if (buffer.type() == TokenType.Null)
            {

                token.next();
                return new NullVariabel();
            }
            else if (buffer.type() == TokenType.LeftBoks)
            {
                //this is a array :)
                return initArray(parse);
            }
            else if (buffer.type() == TokenType.Variabel)
            {
                //control if the next is a assign token =
                if (token.next().type() == TokenType.Assigen)
                {
                    token.next();
                    if (parse)
                        return data.VariabelDatabase.push(buffer.ToString(), handleAfterVariabel(getBooleanPrefix(true), true));
                    else
                        return handleAfterVariabel(getBooleanPrefix(false), false);
                }
                
                
                //control if wee got a variabel this this name
                if (parse && !data.VariabelDatabase.isExists(buffer.ToString()))
                    throw new ScriptError("Unknown variabel: " + buffer.ToString(), buffer.posision());

                Posision p = token.getCache().posision();
                CVar varCache = null;

                if (token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();
                    if (parse)
                    {
                        data.VariabelDatabase.push(buffer.ToString(), varCache = new IntVariabel(data.VariabelDatabase.get(buffer.ToString()).toInt(p)+1));
                    }
                }else if(token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();
                    if (parse)
                    {
                        data.VariabelDatabase.push(buffer.ToString(), varCache = new IntVariabel(data.VariabelDatabase.get(buffer.ToString()).toInt(p) - 1));
                    }

                }

                return handleAfterVariabel(parse ? varCache != null ? varCache : data.VariabelDatabase.get(buffer.ToString()) : new NullVariabel(), parse);
            }
            else if (buffer.type() == TokenType.New)
            {
                //wee try to create a new object :)
                if (token.next().type() != TokenType.Variabel)
                    throw new ScriptError("After 'new' there must be a variabel", token.getCache().posision());
                CVar c = null;
                if (parse)
                {
                    if (!data.VariabelDatabase.isExists(token.getCache().ToString()))
                        throw new ScriptError("Unknown class: " + token.getCache().ToString(), token.getCache().posision());

                    c = data.VariabelDatabase.get(token.getCache().ToString());

                    if (!(c is ClassVariabel))
                        throw new ScriptError(token.getCache().ToString() + " is not a class", token.getCache().posision());
                }

                ClassVariabel cObject = (c != null ? (ClassVariabel)c : null);

                if (token.next().type() != TokenType.LeftBue)
                    throw new ScriptError("Missing ( after class variabel", token.getCache().posision());

                CallAgumentStack call = new CallAgumentStack();
                VariabelDatabase vd = data.VariabelDatabase.createShadow();

                if (token.next().type() != TokenType.RightBue)
                {
                    CVar v = getBoolean(parse);
                    if (parse && cObject.hasConstructor() && cObject.getConstructor().Aguments.size() > 0)
                    {
                        if (cObject.getConstructor().Aguments.get(0).hasType() && !CallScriptFunction.compare(v, cObject.getConstructor().Aguments.get(0).Type))
                        {
                            throw new ScriptError("A agument number 0 in constructor can not be convertet to " + cObject.getConstructor().Aguments.get(0).Type.ToString(), token.getCache().posision());
                        }
                        call.push(v);
                        vd.push(cObject.getConstructor().Aguments.get(0).Name, v);
                    }

                    int i = 1;
                    while (token.getCache().type() == TokenType.Comma)
                    {
                        token.next();
                        v = getBoolean(parse);
                        if (parse && cObject.hasConstructor() && cObject.getConstructor().Aguments.size() >= i)
                        {
                            if (cObject.getConstructor().Aguments.get(i).hasType())
                            {
                                //compare types 
                                if (!CallScriptFunction.compare(v, cObject.getConstructor().Aguments.get(i).Type))
                                    throw new ScriptError("A agument number " + (i + 1) + " in  constructor can not be convertet from '"+v.type()+"' to '" + cObject.getConstructor().Aguments.get(i).Type+"'", token.getCache().posision());
                            }
                            call.push(v);
                            vd.push(cObject.getConstructor().Aguments.get(i).Name, v);
                        }

                        i++;
                    }

                    if (cObject.hasConstructor())
                    {
                        //control if wee missing aguments :)
                        for (int a = i; a < cObject.getConstructor().Aguments.size(); a++)
                        {
                            if (!cObject.getConstructor().Aguments.get(i).hasValue())
                                throw new ScriptError("Call of function  missing aguments", token.getCache().posision());

                            call.push(cObject.getConstructor().Aguments.get(i).Value);
                            vd.push(cObject.getConstructor().Aguments.get(i).Name, cObject.getConstructor().Aguments.get(i).Value);
                        }
                    }
                }

                if (token.getCache().type() != TokenType.RightBue)
                    throw new ScriptError("Missing ) got " + token.getCache().ToString(), token.getCache().posision());
                token.next();

                return handleAfterVariabel(cObject.createNew(call, vd, new EnegyData(vd, new Interprenter(), data.Plugin, data.Error, data)), parse);
            }
            else if (buffer.type() == TokenType.Self) {
                if(parse && data.Interprenter.StaticVariabel == null)
                    throw new ScriptError("'self' can only be uses in static method", buffer.posision());

                if (token.next().type() != TokenType.ObjectBind)
                    return data.Interprenter.StaticVariabel;

                if (token.next().type() != TokenType.Variabel)
                    throw new ScriptError("After 'self->' there must be a variabel", token.getCache().posision());

                string item = token.getCache().ToString();

                if(token.next().type() == TokenType.Assigen)
                {
                    token.next();
                    return assignStaticObject(data.Interprenter.StaticVariabel, item, true, parse);
                }

                return handleStatic(data.Interprenter.StaticVariabel, item, true, parse);
            }
            else if (buffer.type() == TokenType.This)
            {
                if (parse && data.Interprenter.ObjectVariabel == null)
                    throw new ScriptError("'this' can only be used in object method.", buffer.posision());

                //okay now wee control if it return 'this' (the object) or it will get a specifik part :)
                if (token.next().type() != TokenType.ObjectBind)
                {
                    return data.Interprenter.ObjectVariabel;
                }

                //next there must be a variabel :)
                if (token.next().type() != TokenType.Variabel)
                {
                    throw new ScriptError("After 'this->' there must be a variabel", token.getCache().posision());
                }

                string item = token.getCache().ToString();

                if (token.next().type() == TokenType.Assigen)
                {
                    token.next();
                    return assignObject(data.Interprenter.ObjectVariabel, item, parse, true);
                }

                return handleObject(data.Interprenter.ObjectVariabel, item, true, parse);
            }else if(buffer.type() == TokenType.LeftBue)
            {
                token.next();
                CVar result = getBooleanPrefix(parse);
                if(token.getCache().type() != TokenType.RightBue)
                {
                    throw new ScriptError("Missing ). got: "+token.getCache().ToString(), token.getCache().posision());
                }

                token.next();
                return result;
            }else if(buffer.type() == TokenType.EOF)
            {
                return new NullVariabel();
            }
            else
            {
                throw new ScriptError("Unknown token (" + buffer.ToString() + ")", buffer.posision());
            }
        }

        private CVar assignStaticObject(ClassVariabel c, string item, bool isSelf, bool parse)
        {
            if (parse && !c.containsStaticItem(item))
                throw new ScriptError("'" + c.Name + "' do not contain a static item there is been called: " + item, token.getCache().posision());

            if (parse && c.getItem(item).isMethod)
                throw new ScriptError("You can`t append value to static method", token.getCache().posision());

            if (parse && !isSelf && !c.getItem(item).isPublic)
                throw new ScriptError("You can`t use a private static variabel", token.getPosision());

            CVar newValue = handleAfterVariabel(getBooleanPrefix(parse), parse);

            if (parse)
                c.getItem(item).Context = newValue;

            return newValue;
        }

        private CVar assignObject(ObjectVariabel obj, string item, bool parse, bool isThis)
        {
            if(parse && !obj.containsItem(item))
                throw new ScriptError("'" + obj.Name + "' has no item call: " + item, token.getCache().posision());

            if (parse && !obj.isPointer(item))
                throw new ScriptError("You can´t append value to method", token.getCache().posision());

            if(parse && !isThis && !obj.isPublic(item))
                throw new ScriptError("'" + obj.Name + "->" + item + "' is not public", token.getCache().posision());

            CVar newValue = handleAfterVariabel(getBooleanPrefix(parse), parse);

            if (parse)
                obj.appendToPointer(item, newValue);

            return newValue;
        }

        private CVar handleStatic(ClassVariabel c, string item, bool isSelf, bool parse)
        {
            if (parse && !c.containsStaticItem(item))
                throw new ScriptError("'" + c.Name + "' has no static item call: " + item, token.getCache().posision());

            if (parse && !isSelf && !c.getItem(item).isPublic)
                throw new ScriptError("'" + c.Name + "->" + item + "' is not public", token.getCache().posision());

            
            //this is only for pointer :)
            if (!c.getItem(item).isMethod)
            {
                if(token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();

                    if (parse)
                    {
                        c.getItem(item).Context = new IntVariabel(c.getItem(item).Context.toInt(token.getCache().posision()) + 1);
                    }
                }else if(token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();

                    if (parse)
                    {
                        c.getItem(item).Context = new IntVariabel(c.getItem(item).Context.toInt(token.getCache().posision()) - 1);
                    }
                }
            }

            return handleAfterVariabel(parse ? c.getItem(item).Context : new NullVariabel(), parse);
        }
        
        private CVar handleObject(ObjectVariabel obj, string item, bool isThis, bool parse)
        {
            if (parse && !obj.containsItem(item))
                throw new ScriptError("'" + obj.Name + "' has no item call: " + item, token.getCache().posision());

            if (parse && !isThis && !obj.isPublic(item))
                throw new ScriptError("'" + obj.Name + "->" + item + "' is not public", token.getCache().posision());

            //this is only for pointer :)
            if (obj.isPointer(item))
            {
                if (token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();

                    if (parse)
                    {
                        obj.appendToPointer(item, new IntVariabel(obj.get(item).toInt(token.getCache().posision()) + 1));
                    }
                }
                else if (token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();

                    if (parse)
                    {
                        obj.appendToPointer(item, new IntVariabel(obj.get(item).toInt(token.getCache().posision()) + 1));
                    }
                }
            }

            return handleAfterVariabel(parse ? obj.get(item) : new NullVariabel(), parse);
        }

        private CVar initArray(bool parse)
        {
            ArrayVariabel array = new ArrayVariabel();

            if(token.next().type() != TokenType.RightBoks)
            {
                setArrayContext(array, parse);
                while(token.getCache().type() == TokenType.Comma)
                {
                    token.next();
                    setArrayContext(array, parse);
                }
            }

            if(token.getCache().type() != TokenType.RightBoks)
            {
                throw new ScriptError("Missing ] got " + token.getCache().ToString(), token.getCache().posision());
            }

            token.next();

            return array;
        }

        private void setArrayContext(ArrayVariabel array, bool parse)
        {
            CVar first = getBooleanPrefix(parse);
            CVar key;
            CVar value;

            //if there are : in next 
            if(token.getCache().type() == TokenType.DublePunk)
            {
                token.next();
                key = first;//it is the same :)
                value = getBooleanPrefix(parse);
            }
            else
            {
                value = first;
                key   = new IntVariabel(array.getNextID());
            }
            array.put(key, value, token.getCache().posision());
        }

        private CVar handleAfterVariabel(CVar variabel, bool parse)
        {

            //here wee control whe thinks there happend after variabel :)
            switch(token.getCache().type())
            {
                case TokenType.LeftBue://it is a functions call :)
                    VariabelDatabase vd = data.VariabelDatabase.createShadow();

                    if(parse && !(variabel is FunctionVariabel))
                    {
                        throw new ScriptError("Unknown token (", token.getCache().posision());
                    }

                    CallAgumentStack call = new CallAgumentStack();
                    AgumentStack stack = parse ? ((FunctionVariabel)variabel).getStatck() : new AgumentStack();

                    if(token.next().type() != TokenType.RightBue)
                    {
                        CVar v = getBooleanPrefix(parse);
                        if (parse && stack.size() > 0)
                        {
                            if (parse && stack.get(0).hasType() && !CallScriptFunction.compare(v, stack.get(0).Type))
                            {
                                throw new ScriptError("A agument number 0 in function can not be convertet from '"+v.type()+"' to " + stack.get(0).Type.ToString(), token.getCache().posision());
                            }
                            call.push(v);
                            vd.push(stack.get(0).Name, v);
                        }

                        int i = 1;
                        while(token.getCache().type() == TokenType.Comma)
                        {
                            token.next();
                            v = getBooleanPrefix(parse);
                            if (parse && stack.size() >= i)
                            {
                                if (stack.get(i).hasType())
                                {
                                    //compare types 
                                    if (!CallScriptFunction.compare(v, stack.get(i).Type))
                                        throw new ScriptError("A agument number "+(i+1)+" in function can not be convertet to "+ stack.get(i).Type.ToString(), token.getCache().posision());
                                }
                                call.push(v);
                                vd.push(stack.get(i).Name, v);
                            }

                            i++;
                        }

                        //control if wee missing aguments :)
                        for (int a = i; a < stack.size(); a++)
                        {
                            if(!stack.get(a).hasValue())
                                throw new ScriptError("Call of function missing aguments", token.getCache().posision());

                            call.push(stack.get(a).Value);
                            vd.push(stack.get(a).Name, stack.get(a).Value);
                        }
                    }
                    else
                    {
                        //okay wee need to control if it taks 0 aguments :)
                        if(stack.size() != 0)
                        {
                            //it taks aguments :) let see if one ore more requerie aguments :)
                            for(int i = 0; i < stack.size(); i++)
                            {
                                if(!stack.get(i).hasValue())
                                    throw new ScriptError("Call of function missing aguments", token.getCache().posision());

                                vd.push(stack.get(i).Name, stack.get(i).Value);
                                call.push(stack.get(i).Value);
                            }
                        }
                    }

                    if (token.getCache().type() != TokenType.RightBue)
                        throw new ScriptError("Missing ) got "+token.getCache().ToString(), token.getCache().posision());
                    token.next();
                    if (!parse)
                        return handleAfterVariabel(new NullVariabel(), false);

                    return handleAfterVariabel(((FunctionVariabel)variabel).call(call, new EnegyData(vd, new Interprenter(), data.Plugin, data.Error, data)), parse);
                case TokenType.LeftBoks:
                    //control if the context is variabel :)
                    if (!(variabel is ArrayVariabel))
                        throw new ScriptError("You can´t use [ on non array!", token.getCache().posision());

                    //control if the next is ] else get the key :)
                    CVar key = null;

                    if(token.next().type() != TokenType.RightBoks)
                    {
                        //okay it was not ]
                        key = getBooleanPrefix(parse);
                    }

                    if (token.getCache().type() != TokenType.RightBoks)
                        throw new ScriptError("Missing ]. got: "+token.getCache().ToString(), token.getCache().posision());

                    //okay let see if it assign or just get the context :)
                    if(token.next().type() == TokenType.Assigen)
                    {
                        token.next();
                        CVar context = getBooleanPrefix(parse);

                        if (parse)
                        {
                            ((ArrayVariabel)variabel).put((key == null ? new IntVariabel(((ArrayVariabel)variabel).getNextID()) : key), context, token.getCache().posision());
                        }

                        return context;
                    }

                    //okay it is not a assign so let try to see if wee got a key
                    if (key == null)
                        throw new ScriptError("You need to give a key when you tray to get a array value", token.getCache().posision());

                    ArrayVariabel ar = (ArrayVariabel)variabel;

                    if (parse && !ar.keyExists(key, token.getCache().posision()))
                        throw new ScriptError("Unknown key in array: " + key.toString(token.getCache().posision()), token.getCache().posision());
                    if(parse)
                        return handleAfterVariabel(ar.get(key, token.getCache().posision()), parse);

                    return handleAfterVariabel(new NullVariabel(), false);
                case TokenType.ObjectBind:
                    if(parse && !(variabel is ObjectVariabel) && !(variabel is ClassVariabel))
                        throw new ScriptError("Unknown ->", token.getCache().posision());

                    //next there must be a variabel :)
                    if (token.next().type() != TokenType.Variabel)
                        throw new ScriptError("After '->' there must be a variabel", token.getCache().posision());

                    string item = token.getCache().ToString();

                    if(token.next().type() == TokenType.Assigen)
                    {
                        token.next();
                        if (variabel is ClassVariabel)
                            assignStaticObject(parse ? (ClassVariabel)variabel : new ClassVariabel(new Class("")), item, false, parse);
                        else
                            assignObject(parse ? (ObjectVariabel)variabel : new ObjectVariabel(""), item, parse, false);
                    }

                    if(variabel is ClassVariabel)
                        return handleStatic(parse ? (ClassVariabel)variabel : new ClassVariabel(new Class("")), item, false, parse);

                    return handleObject(parse ? (ObjectVariabel)variabel : new ObjectVariabel(""), item, false, parse);
            }

            return variabel;
        }
    }
}
