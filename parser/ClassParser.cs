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
        private bool isFile;

        public ClassVariabel parseNoName(EnegyData data, VariabelDatabase db, Token token)
        {
            return (ClassVariabel)p(data, db, token, false, false);
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            return p(ed, db, token, isFile, true);
        }

        private CVar p(EnegyData ed, VariabelDatabase db, Token token, bool isFile, bool name) { 
            this.isFile = isFile;
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


            if(token.next().type() != TokenType.LeftTuborg)
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
                if (isFile)
                {
                    builder.extraVariabelDatabase = db;
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
            }else if(token.getCache().type() == TokenType.Variabel && token.getCache().ToString() == builder.Name)
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
                data.setError(new ScriptError("1 Unknown token in class (" + builder.Name + ") body: " + token.getCache().ToString() + " | " + token.getCache().type().ToString(), token.getCache().posision()), db);
                return false;
            }

            string name = token.getCache().ToString();
            CVar value = new NullVariabel();

            if(token.next().type() == TokenType.Assigen)
            {
                token.next();
                VariabelParser p = new VariabelParser();
                value = p.parse(data, db, token, isFile);
            }else if(token.getCache().type() == TokenType.End)
            {
                token.next();
            }
            else
            {
                data.setError(new ScriptError("2 Unknown token in class ("+builder.Name+") body: " + token.getCache().ToString(), token.getCache().posision()), db);
                return false;
            }

            builder.addVariabel(name, value, isStatic, isPublic);
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

            if (isStatic)
                buildStaticMethod(isPublic, token, data, type);
            else
                buildNonStaticMethod(isPublic, token, data, type);
            return true;
        }

        public void buildStaticMethod(bool isPublic, Token token, EnegyData data, string type)
        {
            ClassStaticMethods m = new ClassStaticMethods(builder, token.getCache().ToString(), isPublic, type);
            token.next();
            m.Aguments = AgumentParser.parseAguments(token, db, data);
            m.caller += new CallScriptMethod(m.Aguments, BodyParser.parse(token, data, db)).call;
            m.create();
            token.next();
        }

        public void buildNonStaticMethod(bool isPublic, Token token, EnegyData data, string type) {
            ClassMethods m = new ClassMethods(builder, token.getCache().ToString(), isPublic, type);
            token.next();
            m.Aguments = AgumentParser.parseAguments(token, db, data);
            m.caller += new CallScriptMethod(m.Aguments, BodyParser.parse(token, data, db)).call;
            m.create();
            token.next();
        }

        private bool buildConstructor(Token token, EnegyData data)
        {
            ClassMethods cm = new ClassMethods(builder, null, true, null);
            token.next();
            cm.Aguments = AgumentParser.parseAguments(token, db, data);
            cm.caller += new CallScriptMethod(cm.Aguments, BodyParser.parse(token, data, db)).call;
            cm.createConstructor();
            token.next();
            return true;
        }
    }
}
