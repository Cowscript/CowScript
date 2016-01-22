using System;
using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class WhileParser : ParserInterface
    {
        public void end()
        {
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() != TokenType.LeftBue)
                throw new ScriptError("Missing ( after while", token.getCache().posision());
            
            TokenCache scope = ScopeParser.getScope(token);
            ArrayList body = BodyParser.parse(token);
            token.next();

            //run the code until a boolean false i hit :)
            while (ed.State == RunningState.Normal && new VariabelParser().parse(ed, db, scope).toBoolean(new Posision(0, 0)))
            {
                scope.reaset();
                Interprenter.parse(new TokenCache(body), ed, db);
            }

            return new NullVariabel();
        }
    }
}
