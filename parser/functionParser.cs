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

        public void end()
        {
            
        }

        public CVar parse(EnegyData ed, Token token)
        {
            //after function there must be a function name :)
            if(token.next().type() != TokenType.Variabel)
            {
                throw new ScriptError("After function there must be a function name", token.getCache().posision());
            }

            function.Name = token.getCache().ToString();//cache the function name :)

            //let see if wee need to get aguments :)
            AgumentParser.parseAguments(token, function.agument, ed.VariabelDatabase, ed.Interprenter, ed.Error);//wee dont need to put the pointer to ( becuse the agument parse do it for us :)
            //okay now wee has aguments now wee ned to parse body :) 
            function.call = new CallScriptFunction(BodyParser.parse(token), ed.Plugin);

            token.next();
            
            ed.VariabelDatabase.pushFunction(function);
            return null;
        }
    }
}
