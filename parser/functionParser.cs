using script.token;
using script.variabel;
using script.builder;
using script.help;
using script.stack;
using script.plugin.File;

namespace script.parser
{
    class functionParser : ParserInterface
    {
        public CVar parseNoName(EnegyData data, VariabelDatabase db, Token token)
        {
            return parseFunction(data, db, token, false, false, false);
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            return parseFunction(ed, db, token, isFile, true, false);
        }

        public CVar parseFunction(EnegyData ed, VariabelDatabase db, Token token, bool isFile, bool name, bool isPublic)
        { 
            Function function = new Function();

            if (name)
            {
                //after function there must be a function name :)
                if (token.next().type() != TokenType.Variabel)
                {
                    ed.setError(new ScriptError("After function there must be a function name", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                //control if the variabel is type :)
                if (db.isType(token.getCache().ToString()))
                {
                    function.ReturnType = token.getCache().ToString();
                    //it is a type :)
                    if(token.next().type() != TokenType.Variabel)
                    {
                        ed.setError(new ScriptError("After function return type ther must be a name", token.getCache().posision()), db);
                        return new NullVariabel();
                    }

                    function.Name = token.getCache().ToString();
                }
                else {
                    function.Name = token.getCache().ToString();//cache the function name :)
                }

                token.next();
            }
            else
            {
                if(token.next().type() == TokenType.Variabel)
                {
                    if (db.isType(token.getCache().ToString()))
                    {
                        function.ReturnType = token.getCache().ToString();
                        token.next();
                    }
                    else
                    {
                        ed.setError(new ScriptError("Unknown type: "+token.getCache().ToString(), token.getCache().posision()), db);
                        return new NullVariabel();
                    }
                }
            }

            //let see if wee need to get aguments :)
            function.agument = AgumentParser.parseAguments(token, db, ed);//wee dont need to put the pointer to ( becuse the agument parse do it for us :)
            //okay now wee has aguments now wee ned to parse body :) 
            function.call += new CallScriptFunction(BodyParser.parse(token, ed, db), ed.Plugin).call;

            token.next();
            if (name)
            {
                db.pushFunction(function, ed);
                if (db is FileVariabelDatabase && isPublic)
                {
                    function.extraVariabelDatabase = db;
                    ((FileVariabelDatabase)db).VariabelDatabase.pushFunction(function, ed);
                }
                return null;
            }
            else
            {
                return new FunctionVariabel(function);
            }
        }
    }
}
