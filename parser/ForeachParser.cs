using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class ForeachParser : ParserInterface
    {
        public void end()
        {}

        public CVar parse(EnegyData ed, Token token)
        {
            VariabelParser parser = new VariabelParser();

            if (token.next().type() != TokenType.LeftBue)
                throw new ScriptError("Missing ( after foreach", token.getCache().posision());

            token.next();

            CVar agument = parser.parse(ed, token);

            if (!(agument is ArrayVariabel))
                throw new ScriptError("Foreach first agument must be a array!", token.getCache().posision());

            if (token.getCache().type() != TokenType.As)
                throw new ScriptError("After array in foreach there must be a 'as'", token.getCache().posision());

            if (token.next().type() != TokenType.Variabel)
                throw new ScriptError("After 'as' there must be variabel", token.getCache().posision());

            string firstVariabel = token.getCache().ToString();
            string secondVariabel = null;

            //wee see if the next token is : 
            if(token.next().type() == TokenType.DublePunk)
            {
                if (token.next().type() != TokenType.Variabel)
                    throw new ScriptError("After : in foreach there must be a variabel.", token.getCache().posision());

                secondVariabel = token.getCache().ToString();
                token.next();
            }

            if (token.getCache().type() != TokenType.RightBue)
                throw new ScriptError("Missing ) in end of Foreach scope. got "+token.getCache().ToString(), token.getCache().posision());

            ArrayList body = BodyParser.parse(token);

            ArrayVariabel array = (ArrayVariabel)agument;

            foreach(string key in array.Keys())
            {
                ed.VariabelDatabase.push(firstVariabel, (secondVariabel != null ? new StringVariabel(key) : array.get(key, token.getCache().posision())));
                if (secondVariabel != null)
                    ed.VariabelDatabase.push(secondVariabel, array.get(key, token.getCache().posision()));
                ed.Interprenter.parse(new TokenCache(body));
            }

            token.next();
            return null;
        }
    }
}
