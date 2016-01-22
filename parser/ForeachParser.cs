using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class ForeachParser : ParserInterface
    {
        public void end()
        {}

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            VariabelParser parser = new VariabelParser();

            if (token.next().type() != TokenType.LeftBue)
            {
                ed.setError(new ScriptError("Missing ( after foreach", token.getCache().posision()), db);
                return new NullVariabel();
            }

            token.next();

            CVar agument = parser.parse(ed, db, token);

            if (!(agument is ArrayVariabel))
            {
                ed.setError(new ScriptError("Foreach first agument must be a array!", token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (token.getCache().type() != TokenType.As)
            {
                ed.setError(new ScriptError("After array in foreach there must be a 'as'", token.getCache().posision()), db);
                return new NullVariabel();
            }

            if (token.next().type() != TokenType.Variabel)
            {
                ed.setError(new ScriptError("After 'as' there must be variabel", token.getCache().posision()), db);
                return new NullVariabel();
            }

            string firstVariabel = token.getCache().ToString();
            string secondVariabel = null;

            //wee see if the next token is : 
            if(token.next().type() == TokenType.DublePunk)
            {
                if (token.next().type() != TokenType.Variabel)
                {
                    ed.setError(new ScriptError("After : in foreach there must be a variabel.", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                secondVariabel = token.getCache().ToString();
                token.next();
            }

            if (token.getCache().type() != TokenType.RightBue)
            {
                ed.setError(new ScriptError("Missing ) in end of Foreach scope. got " + token.getCache().ToString(), token.getCache().posision()), db);
                return new NullVariabel();
            }

            ArrayList body = BodyParser.parse(token, ed, db);

            ArrayVariabel array = (ArrayVariabel)agument;

            foreach(string key in array.Keys())
            {
                if (ed.State != RunningState.Normal)
                    break;

                db.push(firstVariabel, (secondVariabel != null ? new StringVariabel(key) : array.get(key, token.getCache().posision(), ed, db)));
                if (secondVariabel != null)
                    db.push(secondVariabel, array.get(key, token.getCache().posision(), ed, db));
                Interprenter.parse(new TokenCache(body), ed, db);
            }

            token.next();
            return null;
        }
    }
}
