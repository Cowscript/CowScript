using System;
using script.token;
using script.variabel;
using script.builder;
using script.help;
using script.stack;
using System.Globalization;

namespace script.parser
{
    class VariabelParser : ParserInterface
    {
        private EnegyData data;
        private Token token;
        private VariabelDatabase db;

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            CVar result = parseNoEnd(ed, db, token);

            if (token.getCache().type() != TokenType.End && ed.State == RunningState.Normal)
            {
                ed.setError(new ScriptError("Missing ;. got: " + token.getCache().ToString(), token.getCache().posision()), db);
                return new NullVariabel();
            }

            token.next();
            return result;
        }

        public CVar parseNoEnd(EnegyData ed, VariabelDatabase db, Token token)
        {
            this.token = token;
            this.db = db;
            data = ed;

            return getAsk(true);
        }

        public CVar getAsk(bool parse)
        {
            CVar var = getBooleanPrefix(parse);

            if(token.getCache().type() == TokenType.Ask)
            {
                bool befor = false;
                token.next();
                if(parse && var.toBoolean(token.getCache().posision(), data, db))
                {
                    befor = true;
                    var = parseNoEnd(data, db, token);
                }
                else
                {
                    getAsk(false);
                }

                if(parse && token.getCache().type() != TokenType.DublePunk)
                {
                    data.setError(new ScriptError("Missing : in ? node", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                token.next();

                if(parse && !befor)
                {
                    var = parseNoEnd(data, db, token);
                }
                else
                {
                    getAsk(false);
                }
            }

            return var;
        }

        private CVar getBooleanPrefix(bool parse)
        {
            if(token.getCache().type() == TokenType.Not)
            {
                token.next();
                if (parse)
                    return new BooleanVariabel(!getBoolean(true).toBoolean(token.getCache().posision(), data, db));
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
                if (!var.toBoolean(token.getCache().posision(), data, db))
                {
                    //this is not true soo wee use this :)
                    return new BooleanVariabel(getAsk(parse).toBoolean(token.getCache().posision(), data, db));
                }

                getAsk(false);//no need this becuse it is true :)
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
                if (!var.toBoolean(token.getCache().posision(), data, db))
                {
                    getBooleanAnd(false);
                    return new BooleanVariabel(false);//wee dont use this :)
                }

                return new BooleanVariabel(getBooleanAnd(parse).toBoolean(token.getCache().posision(), data, db));
            }

            return var;
        }

        private CVar getBooleanFactor(bool parse)
        {
            CVar var = booleanRelation(parse);

            if(token.getCache().type() == TokenType.Not)
            {
                token.next();
                return new BooleanVariabel(!booleanRelation(parse).toBoolean(token.getCache().posision(), data, db));
            }

            return var;
        }

        private CVar booleanRelation(bool parse)
        {
            CVar var = sum(parse);

            if (token.getCache().type() == TokenType.Equels) //==
            {
                token.next();
                return new BooleanVariabel(sum(parse).compare(var, token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.NotEquels) //!=
            {
                token.next();
                return new BooleanVariabel(!sum(parse).compare(var, token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.GreaterEquels) // <=
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision(), data, db) >= var.toInt(token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.Greater)
            { //<
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision(), data, db) > var.toInt(token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.LessEquels)
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision(), data, db) <= var.toInt(token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.Less)
            {
                token.next();
                return new BooleanVariabel(sum(parse).toInt(token.getCache().posision(), data, db) < var.toInt(token.getCache().posision(), data, db));
            }
            else if (token.getCache().type() == TokenType.Is)
            {
                token.next();
                CVar next = sum(parse);

                if (parse && !(var is ClassVariabel || next is ClassVariabel))
                {
                    data.setError(new ScriptError("The node befor is or after must be class identify", token.getCache().posision()), db);
                    return new BooleanVariabel(false);
                }

                if (parse)
                {
                    ClassVariabel c = (var is ClassVariabel ? (ClassVariabel)var : (ClassVariabel)next);
                    ObjectVariabel o = (var is ClassVariabel ? (ObjectVariabel)next : (ObjectVariabel)var);

                    return new BooleanVariabel(c.Name == o.Name);
                }

                return new BooleanVariabel(false);
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
                    if (IntVariabel.isInt(b) && IntVariabel.isInt(var))
                        var = IntVariabel.createInt(data, db, buffer.posision(), var.toInt(buffer.posision(), data, db) + b.toInt(token.getCache().posision(), data, db));
                    else
                        var = StringVariabel.CreateString(data, db, token.getCache().posision(), var.toString(buffer.posision(), data, db) + b.toString(token.getCache().posision(), data, db));
                }
                else
                {
                    var = IntVariabel.createInt(data, db, buffer.posision(), var.toInt(buffer.posision(), data, db) - gange(parse).toInt(token.getCache().posision(), data, db));
                }
            }

            return var;
        }

        private CVar gange(bool parse)
        {
            CVar var = power(parse);

            TokenBuffer buffer;
            while((buffer = token.getCache()).type() == TokenType.Gange || buffer.type() == TokenType.Divider)
            {
                token.next();
                if(buffer.type() == TokenType.Gange)
                {
                    var = IntVariabel.createInt(data, db, token.getCache().posision(), var.toInt(buffer.posision(), data, db) * power(parse).toInt(token.getCache().posision(), data, db));
                }
                else
                {
                    var = IntVariabel.createInt(data, db, token.getCache().posision(), var.toInt(buffer.posision(), data, db) / power(parse).toInt(token.getCache().posision(), data, db));
                }
            }

            return var;
        }

        private CVar power(bool parse)
        {
            CVar var = negetave(parse);
            if(token.getCache().type() == TokenType.Power)
            {
                token.next();
                if (parse)
                {
                    var = IntVariabel.createInt(data, db, token.getCache().posision(), Math.Pow(var.toInt(token.getCache().posision(), data, db), power(parse).toInt(token.getCache().posision(), data, db)));
                }
                else
                    negetave(parse);
            }

            return var;
        }

        private CVar negetave(bool parse)
        {
            
            if(token.getCache().type() == TokenType.Minus)
            {
                token.next();
                if (parse)
                    return IntVariabel.createInt(data, db, token.getCache().posision(), -atom(parse).toInt(token.getCache().posision(), data, db));
                return atom(parse);
            }else if(token.getCache().type() == TokenType.Plus)
            {
                token.next();
                if (parse)
                    return IntVariabel.createInt(data, db, token.getCache().posision(), +atom(parse).toInt(token.getCache().posision(), data, db));
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
                return StringVariabel.CreateString(data, db, buffer.posision(), buffer.ToString());
            }
            else if (buffer.type() == TokenType.Int)
            {
                token.next();
                return IntVariabel.createInt(data, db, buffer.posision(), double.Parse(buffer.ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US")));
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
                        return db.push(buffer.ToString(), handleAfterVariabel(getBooleanPrefix(true), true), data);
                    else
                        return handleAfterVariabel(getBooleanPrefix(false), false);
                }


                //control if wee got a variabel this this name
                if (parse && !db.isExists(buffer.ToString()))
                {
                    data.setError(new ScriptError("Unknown variabel: " + buffer.ToString(), buffer.posision()), db);
                    return new NullVariabel();
                }

                Posision p = token.getCache().posision();
                CVar varCache = null;

                if (token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();
                    if (parse)
                    {
                        db.push(buffer.ToString(), varCache = IntVariabel.createInt(data, db, token.getCache().posision(), db.get(buffer.ToString(), data).toInt(p, data, db) + 1), data);
                    }
                }else if(token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();
                    if (parse)
                    {
                        db.push(buffer.ToString(), varCache = IntVariabel.createInt(data, db, token.getCache().posision(), db.get(buffer.ToString(), data).toInt(p, data, db) - 1), data);
                    }

                }

                return handleAfterVariabel(parse ? varCache != null ? varCache : db.get(buffer.ToString(), data) : new NullVariabel(), parse);
            }
            else if (buffer.type() == TokenType.New)
            {
                ClassVariabel cObject = null;
                if (token.next().type() == TokenType.Class)
                {
                    cObject = new ClassParser().parseNoName(data, db, token);
                }else if(token.getCache().type() == TokenType.Variabel)
                {
                    if (!db.isExists(token.getCache().ToString()))
                    {
                        data.setError(new ScriptError("Unknown class '" + token.getCache().ToString(), token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    //control it is a class variabel :)
                    CVar cache = db.get(token.getCache().ToString(), data);

                    if(!(cache is ClassVariabel))
                    {
                        data.setError(new ScriptError("Unknown class '" + token.getCache().ToString(), token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    cObject = (ClassVariabel)cache;

                    token.next();
                }
                else
                {
                    data.setError(new ScriptError("Unknown token after 'new': " + token.getCache().ToString(), token.getCache().posision()), db);
                    return new NullVariabel();
                }

                if (token.getCache().type() != TokenType.LeftBue)
                {
                    data.setError(new ScriptError("Missing ( after class variabel. got: "+token.getCache().ToString(), token.getCache().posision()), db);
                    return new NullVariabel();
                }
                ParseFunctionCallResult pfcr;

                if (parse && cObject.hasConstructor())
                    pfcr = CallScriptFunction.parseCall(parse, cObject.getConstructor().Aguments, new VariabelDatabase(), token, this, data, cObject.getConstructor().setVariabel);
                else
                    pfcr = CallScriptFunction.parseCall(parse, new AgumentStack(), db, token, this, data, false);

                return handleAfterVariabel(cObject.createNew(pfcr.Call, db, data, token.getCache().posision()), parse);
            }
            else if (buffer.type() == TokenType.Self) {
                if (parse && db.C == null)
                {
                    data.setError(new ScriptError("'self' can only be uses in static method", buffer.posision()), db);
                    return new NullVariabel();
                }

                if (token.next().type() != TokenType.ObjectBind)
                    return db.C;

                if (token.next().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("After 'self->' there must be a variabel", token.getCache().posision()), db);
                    return new NullVariabel();
                }
                string item = token.getCache().ToString();

                if(token.next().type() == TokenType.Assigen)
                {
                    token.next();
                    return assignStaticObject(db.C, item, true, parse);
                }

                return handleStatic(db.C, item, true, parse);
            }
            else if (buffer.type() == TokenType.This)
            {
                if (parse && db.Object == null)
                {
                    data.setError(new ScriptError("'this' can only be used in object method.", buffer.posision()), db);
                    return new NullVariabel();
                }

                //okay now wee control if it return 'this' (the object) or it will get a specifik part :)
                if (token.next().type() != TokenType.ObjectBind)
                {
                    return db.Object;
                }

                //next there must be a variabel :)
                if (token.next().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("After 'this->' there must be a variabel", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                string item = token.getCache().ToString();

                if (token.next().type() == TokenType.Assigen)
                {
                    token.next();
                    return assignObject(db.Object, item, parse, true);
                }

                return handleObject(db.Object, item, true, parse);
            }else if(buffer.type() == TokenType.LeftBue)
            {
                token.next();
                CVar result = getBooleanPrefix(parse);
                if(token.getCache().type() != TokenType.RightBue)
                {
                    data.setError(new ScriptError("Missing ). got: "+token.getCache().ToString(), token.getCache().posision()), db);
                    return new NullVariabel();
                }

                token.next();
                return result;
            }else if(buffer.type() == TokenType.EOF)
            {
                return new NullVariabel();
            }else if(buffer.type() == TokenType.Function)
            {
                return new functionParser().parseNoName(data, db, token);
            }else if(buffer.type() == TokenType.Class)
            {
                return new ClassParser().parseNoName(data, db, token);
            }
            else
            {
                data.setError(new ScriptError("Unknown token (" + buffer.ToString() + ")", buffer.posision()), db);
                return new NullVariabel();
            }
        }

        private CVar assignStaticObject(ClassVariabel c, string item, bool isSelf, bool parse)
        {
            if (parse && !c.containsStaticItem(item))
            {
                data.setError(new ScriptError("'" + c.Name + "' do not contain a static item there is been called: " + item, token.getCache().posision()), db);
                return new NullVariabel();
            }


            if (parse && c.getItem(item).isMethod)
            {
                 data.setError(new ScriptError("You can`t append value to static method", token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (parse && !isSelf && !c.getItem(item).isPublic)
            {
                data.setError(new ScriptError("You can`t use a private static variabel", token.getCache().posision()), db);
                return new NullVariabel();

            }

            CVar newValue = handleAfterVariabel(getBooleanPrefix(parse), parse);

            if (parse)
                c.getItem(item).Context = newValue;

            return newValue;
        }

        private CVar assignObject(ObjectVariabel obj, string item, bool parse, bool isThis)
        {
            if (parse && !obj.containsItem(item))
            {
                data.setError(new ScriptError("'" + obj.Name + "' has no item call: " + item, token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (parse && !obj.isPointer(item))
            {
                data.setError(new ScriptError("You can´t append value to method", token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (parse && !isThis && !obj.isPublic(item))
            {
                data.setError(new ScriptError("'" + obj.Name + "->" + item + "' is not public", token.getCache().posision()), db);
                return new NullVariabel();
            }

            CVar newValue = handleAfterVariabel(getBooleanPrefix(parse), parse);

            if (parse)
                obj.appendToPointer(item, newValue);

            return newValue;
        }

        private CVar handleStatic(ClassVariabel c, string item, bool isSelf, bool parse)
        {
            if (parse && !c.containsStaticItem(item))
            {
                data.setError(new ScriptError("'" + c.Name + "' has no static item call: " + item, token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (parse && !isSelf && !c.getItem(item).isPublic)
            {
                data.setError(new ScriptError("'" + c.Name + "->" + item + "' is not public", token.getCache().posision()), db);
                return new NullVariabel();
            }

            
            //this is only for pointer :)
            if (!c.getItem(item).isMethod)
            {
                if(token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();

                    if (parse)
                    {
                        c.getItem(item).Context = IntVariabel.createInt(data, db, token.getCache().posision(), c.getItem(item).Context.toInt(token.getCache().posision(), data, db) + 1);
                    }
                }else if(token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();

                    if (parse)
                    {
                        c.getItem(item).Context = IntVariabel.createInt(data, db, token.getCache().posision(), c.getItem(item).Context.toInt(token.getCache().posision(), data, db) - 1);
                    }
                }
            }

            return handleAfterVariabel(parse ? c.getItem(item).Context : new NullVariabel(), parse);
        }
        
        private CVar handleObject(ObjectVariabel obj, string item, bool isThis, bool parse)
        {
            if (parse && !obj.containsItem(item))
            {
                data.setError(new ScriptError("'" + obj.Name + "' has no item call: " + item, token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (parse && !isThis && !obj.isPublic(item))
            {
                data.setError(new ScriptError("'" + obj.Name + "->" + item + "' is not public", token.getCache().posision()), db);
                return new NullVariabel();
            }

            //this is only for pointer :)
            if (parse && obj.isPointer(item))
            {
                if (token.getCache().type() == TokenType.PlusOne)
                {
                    token.next();

                    if (parse)
                    {
                        obj.appendToPointer(item, IntVariabel.createInt(data, db, token.getCache().posision(), obj.get(item).toInt(token.getCache().posision(), data, db) + 1));
                    }
                }
                else if (token.getCache().type() == TokenType.MinusOne)
                {
                    token.next();

                    if (parse)
                    {
                        obj.appendToPointer(item, IntVariabel.createInt(data, db, token.getCache().posision(), obj.get(item).toInt(token.getCache().posision(), data, db) + 1));
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
                data.setError(new ScriptError("Missing ] got " + token.getCache().ToString(), token.getCache().posision()), db);
                return new NullVariabel();
            }

            token.next();

            return handleAfterVariabel(array, parse);
        }

        private void setArrayContext(ArrayVariabel array, bool parse)
        {
            CVar first = getBooleanPrefix(parse);

            //if there are : in next 
            if(token.getCache().type() == TokenType.DublePunk)
            {
                token.next();
                array.put(first, getBooleanPrefix(parse), token.getCache().posision(), data, db);
            }
            else
            {
                array.put(first, token.getCache().posision(), data, db);
            }
        }

        private CVar handleAfterVariabel(CVar variabel, bool parse)
        {
            Posision startPos = token.getCache().posision();

            //here wee control whe thinks there happend after variabel :)
            switch(token.getCache().type())
            {
                case TokenType.LeftBue://it is a functions call :)
                    ParseFunctionCallResult pfcr;
                    if (parse)
                    {
                        if(!(variabel is FunctionVariabel))
                        {
                            data.setError(new ScriptError("Variabel befor ( must be a function, method or class identify", token.getCache().posision()), db);
                            return new NullVariabel();
                        }

                        pfcr = CallScriptFunction.parseCall(parse, ((FunctionVariabel)variabel).getStatck(), ((FunctionVariabel)variabel).getShadowVariabelDatabase(db), token, this, data, ((FunctionVariabel)variabel).SetVariabel);
                    }
                    else
                    {
                        pfcr = CallScriptFunction.parseCall(parse, new AgumentStack(), db, token, this, data, false);
                    }

                    if(pfcr == null || !parse)
                    {
                        return handleAfterVariabel(new NullVariabel(), false);
                    }

                    CVar r = ((FunctionVariabel)variabel).call(pfcr.Call, pfcr.VariabelDatabase, data, token.getCache().posision());

                    if (data.State != RunningState.Normal)
                        return new NullVariabel();

                    return handleAfterVariabel(r, parse);
                case TokenType.LeftBoks:
                    //control if the context is variabel :)
                    if (!(variabel is ArrayVariabel))
                    {
                        data.setError(new ScriptError("You can´t use [ on non array!", token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    //control if the next is ] else get the key :)
                    CVar key = null;

                    if(token.next().type() != TokenType.RightBoks)
                    {
                        //okay it was not ]
                        key = getBooleanPrefix(parse);
                    }

                    if (token.getCache().type() != TokenType.RightBoks)
                    {
                        data.setError(new ScriptError("Missing ]. got: " + token.getCache().ToString(), token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    //okay let see if it assign or just get the context :)
                    if(token.next().type() == TokenType.Assigen)
                    {
                        token.next();
                        CVar context = getBooleanPrefix(parse);

                        if (parse)
                        {
                            ((ArrayVariabel)variabel).put((key == null ? ((ArrayVariabel)variabel).getNextID(data, db, token.getCache().posision()) : key), context, token.getCache().posision(), data, db);
                        }

                        return context;
                    }

                    //okay it is not a assign so let try to see if wee got a key
                    if (key == null)
                    {
                        data.setError(new ScriptError("You need to give a key when you tray to get a array value", token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    ArrayVariabel ar = (ArrayVariabel)variabel;

                    if (parse && !ar.keyExists(key, token.getCache().posision(), data, db))
                    {
                        data.setError(new ScriptError("Unknown key in array: " + key.toString(token.getCache().posision(), data, db), token.getCache().posision()), db);
                    }
                    if(parse)
                        return handleAfterVariabel(ar.get(key, token.getCache().posision(), data, db), parse);

                    return handleAfterVariabel(new NullVariabel(), false);
                case TokenType.ObjectBind:
                    if (parse && !(variabel is ObjectVariabel) && !(variabel is ClassVariabel))
                    {
                        data.setError(new ScriptError("Unknown ->", token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    //next there must be a variabel :)
                    if (token.next().type() != TokenType.Variabel)
                    {
                        data.setError(new ScriptError("After '->' there must be a variabel", token.getCache().posision()), db);
                        return new NullVariabel();
                    }

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
