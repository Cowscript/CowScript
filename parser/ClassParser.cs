using script.builder;
using script.help;
using script.stack;
using script.token;
using script.variabel;

namespace script.parser
{
    class ClassParser : ParserInterface
    {
        private Class builder;

        public void end()
        {}

        public CVar parse(EnegyData ed, Token token)
        {
            if(token.next().type() != TokenType.Variabel)
            {
                throw new ScriptError("Missing class name after 'class'", token.getCache().posision());
            }

            builder = new Class(token.getCache().ToString());

            //control if wee got { after class name
            if(token.next().type() != TokenType.LeftTuborg)
            {
                throw new ScriptError("Missing { after class name", token.getCache().posision());
            }

            token.next();

            while (run(token.getCache().type()))
                build(token, ed);

            if(token.getCache().type() != TokenType.RightTuborg)
            {
                throw new ScriptError("Missing } in end of class building", token.getCache().posision());
            }

            token.next();
            ed.VariabelDatabase.pushClass(builder);
            return null;
        }

        private bool run(TokenType type)
        {
            return type != TokenType.EOF && type != TokenType.RightTuborg;
        }

        private void build(Token token, EnegyData data)
        {
            //here wee control if it private or public. protected is comming when class system is builder more avancrede
            if(token.getCache().type() == TokenType.Public)
            {
                token.next();
                isStatic(true, token, data);
            }else if(token.getCache().type() == TokenType.Private)
            {
                token.next();
                isStatic(false, token, data);
            }else if(token.getCache().type() == TokenType.Variabel && token.getCache().ToString() == builder.Name)
            {
                buildConstructor(token, data);
            }
            else
            {
                isStatic(true, token, data);
            }
        }

        private void isStatic(bool isPublic, Token token, EnegyData data)
        {
            if(token.getCache().type() == TokenType.Static)
            {
                token.next();
                buildBody(isPublic, true, token, data);
            }else
                buildBody(isPublic, false, token, data);
        }

        private void buildBody(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            if (token.getCache().type() == TokenType.Function) //it is a function :)
            {
                token.next();
                buildMethod(isPublic, isStatic, token, data);
            }else
                buildVariabel(isPublic, isStatic, token, data);
        }

        private void buildVariabel(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            //it is a variabel :)
            //okay it is a variabel ?
            if (token.getCache().type() != TokenType.Variabel)
                throw new ScriptError("1 Unknown token in class (" + builder.Name + ") body: " + token.getCache().ToString()+" | "+token.getCache().type().ToString(), token.getCache().posision());

            string name = token.getCache().ToString();
            CVar value = new NullVariabel();

            if(token.next().type() == TokenType.Assigen)
            {
                token.next();
                VariabelParser p = new VariabelParser();
                value = p.parse(new EnegyData(new VariabelDatabase(), data.Interprenter, data.Plugin, data.Error, data), token);
                p.end();
            }else if(token.getCache().type() == TokenType.End)
            {
                token.next();
            }
            else
            {
                throw new ScriptError("2 Unknown token in class ("+builder.Name+") body: " + token.getCache().ToString(), token.getCache().posision());
            }

            builder.addVariabel(name, value, isStatic, isPublic);
        }

        private void buildMethod(bool isPublic, bool isStatic, Token token, EnegyData data)
        {
            //control if there are is name after function :)
            if (token.getCache().type() != TokenType.Variabel)
            {
                throw new ScriptError("A method should have a name", token.getCache().posision());
            }


            if (isStatic)
                buildStaticMethod(isPublic, token, data);
            else
                buildNonStaticMethod(isPublic, token, data);
        }

        public void buildStaticMethod(bool isPublic, Token token, EnegyData data)
        {
            ClassStaticMethods m = (ClassStaticMethods)builder.createMethods(true);
            m.setAccess(isPublic);
            m.setName(token.getCache().ToString());
            m.Aguments = AgumentParser.parseAguments(token, data.VariabelDatabase, data.Interprenter, data.Error);
            m.caller += new CallScriptMethod(m.Aguments, BodyParser.parse(token)).call;
            m.create();
            token.next();
        }

        public void buildNonStaticMethod(bool isPublic, Token token, EnegyData data) {
            ClassMethods m = (ClassMethods)builder.createMethods(false);
            m.setAccess(isPublic);
            m.setName(token.getCache().ToString());
            m.Aguments = AgumentParser.parseAguments(token, data.VariabelDatabase, data.Interprenter, data.Error);
            m.caller += new CallScriptMethod(m.Aguments, BodyParser.parse(token)).call;
            m.create();
            token.next();
        }

        private void buildConstructor(Token token, EnegyData data)
        {
            ClassMethods cm = (ClassMethods)builder.createMethods();
            cm.setName(builder.Name);
            AgumentStack s = AgumentParser.parseAguments(token, data.VariabelDatabase, data.Interprenter, data.Error);
            cm.Aguments = s;
            cm.caller += new CallScriptMethod(s, BodyParser.parse(token)).call;
            cm.createConstructor();
            token.next();
        }
    }
}
