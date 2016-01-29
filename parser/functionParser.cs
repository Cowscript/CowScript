using script.token;
using script.variabel;
using script.builder;
using script.help;
using script.stack;

namespace script.parser
{
    class functionParser : ParserInterface
    {
        private Function function = new Function();

        public void end(EnegyData data, VariabelDatabase db)
        {
            
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            //after function there must be a function name :)
            if(token.next().type() != TokenType.Variabel)
            {
                ed.setError(new ScriptError("After function there must be a function name", token.getCache().posision()), db);
                return new NullVariabel();
            }

            function.Name = token.getCache().ToString();//cache the function name :)

            //let see if wee need to get aguments :)
            function.agument = AgumentParser.parseAguments(token, db, ed);//wee dont need to put the pointer to ( becuse the agument parse do it for us :)
            //okay now wee has aguments now wee ned to parse body :) 
            function.call += new CallScriptFunction(BodyParser.parse(token, ed, db), ed.Plugin).call;

            token.next();
            
            db.pushFunction(function, ed);
            return null;
        }
    }
}
