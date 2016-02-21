using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class WhileParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() != TokenType.LeftBue)
                ed.setError(new ScriptError("Missing ( after while", token.getCache().posision()), db);
            
            TokenCache scope = ScopeParser.getScope(token, ed, db);
            ArrayList body = BodyParser.parse(token, ed, db);
            token.next();

            //run the code until a boolean false i hit :)
            while (ed.State == RunningState.Normal && new VariabelParser().parseNoEnd(ed, db, scope).toBoolean(new Posision(0, 0), ed, db))
            {
                Interprenter.parse(new TokenCache(body, ed, db), ed, db);
                if(ed.State == RunningState.Continue)
                {
                    ed.setNormal();
                }else if(ed.State == RunningState.Break)
                {
                    ed.setNormal();
                    break;
                }
            }

            return new NullVariabel();
        }
    }
}
