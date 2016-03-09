using script.builder;
using script.help;
using script.plugin.File;
using script.stack;
using script.token;
using script.variabel;

namespace script.parser
{
    class ClassParser : ParserInterface
    {
        private Class builder;
        private VariabelDatabase db;

        public ClassVariabel parseNoName(EnegyData data, VariabelDatabase db, Token token)
        {
            return (ClassVariabel)p(data, db, token, false);
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            return p(ed, db, token, true);
        }

        private CVar p(EnegyData ed, VariabelDatabase db, Token token, bool name) {
            this.db = db;

            if (name)
            {
                if (token.next().type() != TokenType.Variabel)
                {
                    ed.setError(new ScriptError("Missing class name after 'class'", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                builder = new Class(token.getCache().ToString());
            }
            else
            {
                builder = new Class();
            }

            if(token.next().type() == TokenType.Extends)
            {
                if(token.next().type() != TokenType.Variabel)
                {
                    ed.setError(new ScriptError("Missing variabel after extends", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                //control if the variabel exteis in the variabel database
                if (!db.isExists(token.getCache().ToString()))
                {
                    ed.setError(new ScriptError("Unknown variabel: " + token.getCache().ToString(), token.getCache().posision()), db);
                    return new NullVariabel();
                }

                CVar buffer = db.get(token.getCache().ToString(), ed);

                if(!(buffer is ClassVariabel))
                {
                    ed.setError(new ScriptError(token.getCache().ToString() + " is not a class", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                builder.Extends((ClassVariabel)buffer);
                token.next();
            }


            if(token.getCache().type() != TokenType.LeftTuborg)
            {
                ed.setError(new ScriptError("Missing { after class name", token.getCache().posision()), db);
                return new NullVariabel();
            }

            token.next();

            while (ed.State == RunningState.Normal && run(token.getCache().type()))
                build(token, ed);

            if(token.getCache().type() != TokenType.RightTuborg)
            {
                ed.setError(new ScriptError("Missing } in end of class building", token.getCache().posision()), db);
                return new NullVariabel();
            }

            token.next();
            if (name)
            {
                db.pushClass(builder, ed);
                if (db is FileVariabelDatabase)
                {
                    builder.SetExtraVariabelDatabase(db);
                    ((FileVariabelDatabase)db).VariabelDatabase.pushClass(builder, ed);
                }
                return null;
            }
            else
            {
                return new ClassVariabel(builder);
            }
        }

        private bool run(TokenType type)
        {
            return type != TokenType.EOF && type != TokenType.RightTuborg;
        }

        private bool build(Token token, EnegyData data)
        {
            //here wee control if it private or public. protected is comming when class system is builder more avancrede
            if(token.getCache().type() == TokenType.Public)
            {
                token.next();
                return isStatic(true, token, data);
            }else if(token.getCache().type() == TokenType.Private)
            {
                token.next();
                return isStatic(false, token, data);
            }else if(token.getCache().type() == TokenType.Variabel && token.getCache().ToString() == builder.GetContainer().Name)
            {
                return buildConstructor(token, data);
            }
            else
            {
                return isStatic(true, token, data);
            }
        }

        private bool isStatic(bool isPublic, Token token, EnegyData data)
        {
            if(token.getCache().type() == TokenType.Static)
            {
                token.next();
                return buildBody(isPublic, true, token, data);
            }else
                return buildBody(isPublic, false, token, data);
        }

        private bool buildBody(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            if (token.getCache().type() == TokenType.Function) //it is a function :)
            {
                token.next();
                return buildMethod(isPublic, isStatic, token, data);
            }else
                return buildVariabel(isPublic, isStatic, token, data);
        }

        private bool buildVariabel(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            //it is a variabel :)
            //okay it is a variabel ?
            if (token.getCache().type() != TokenType.Variabel) {
                data.setError(new ScriptError("1 Unknown token in class (" + builder.GetContainer().Name + ") body: " + token.getCache().ToString() + " | " + token.getCache().type().ToString(), token.getCache().posision()), db);
                return false;
            }

            Pointer pointer = new Pointer(token.getCache().ToString());
            
            if(token.next().type() == TokenType.Assigen)
            {
                token.next();
                pointer.SetDefault(new VariabelParser().parse(data, db, token));
            }else if(token.getCache().type() == TokenType.End)
            {
                token.next();
            }
            else
            {
                data.setError(new ScriptError("2 Unknown token in class ("+builder.GetContainer().Name+") body: " + token.getCache().ToString(), token.getCache().posision()), db);
                return false;
            }


            if (isStatic)
                pointer.SetStatic();

            if (!isPublic)
                pointer.SetPrivate();

            builder.SetPointer(pointer, data, db, token.getCache().posision());
            return true;
        }

        private bool buildMethod(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            //control if there are is name after function :)
            if (token.getCache().type() != TokenType.Variabel)
            {
                data.setError(new ScriptError("A method should have a name", token.getCache().posision()), db);
                return false;
            }

            string type = null;

            //okay the name can be a type so wee control it here :)
            if (db.isType(token.getCache().ToString()))
            {
                type = token.getCache().ToString();

                if(token.next().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("A method should have a name", token.getCache().posision()), db);
                    return false;
                }
            }

            Method method = new Method(token.getCache().ToString());

            token.next();

            if (isStatic)
                method.SetStatic();

            method.SetAgumentStack(AgumentParser.parseAguments(token, db, data));
            method.SetBody(new CallScriptMethod(method.GetAgumentStack(), BodyParser.parse(token, data, db)).call);
            method.SetVariabel();//in this way the script can use agument as name :)

            builder.SetMethod(method, data, db, token.getCache().posision());
            token.next();
            return true;
        }

        private bool buildConstructor(Token token, EnegyData data)
        {
            Method cm = new Method(null);
            token.next();
            cm.SetAgumentStack(AgumentParser.parseAguments(token, db, data));
            cm.SetBody(new CallScriptMethod(cm.GetAgumentStack(), BodyParser.parse(token, data, db)).call);
            cm.SetVariabel();
            builder.SetConstructor(cm, data, db, token.getCache().posision());
            token.next();
            return true;
        }
    }
}
