using System;
using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class WhileParser : ParserInterface
    {
        public void end(EnegyData data, VariabelDatabase db)
        {
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() != TokenType.LeftBue)
                ed.setError(new ScriptError("Missing ( after while", token.getCache().posision()), db);
            
            TokenCache scope = ScopeParser.getScope(token, ed, db);
            ArrayList body = BodyParser.parse(token, ed, db);
            token.next();

            //run the code until a boolean false i hit :)
            while (ed.State == RunningState.Normal && new VariabelParser().parse(ed, db, scope).toBoolean(new Posision(0, 0), ed, db))
            {
                scope.reaset();
                Interprenter.parse(new TokenCache(body, ed, db), ed, db);
            }

            return new NullVariabel();
        }
    }
}
