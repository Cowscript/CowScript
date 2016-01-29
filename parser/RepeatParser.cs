using System;
using script.token;
using script.variabel;

namespace script.parser
{
    //repeat is a new sytrack devopled for CowScript. It look like a functin but missing ;. if th contetext return true it call it agian else it stop
    class RepeatParser : ParserInterface
    {
        public void end(EnegyData data, VariabelDatabase db)
        {}

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() != TokenType.LeftBue)
            {
                ed.setError(new ScriptError("Missing ( after repeat", token.getCache().posision()), db);
                return new NullVariabel();
            }

            TokenCache cache = ScopeParser.getScope(token, ed, db);
            token.next();

            while (new VariabelParser().parse(ed, db, cache).toBoolean(token.getCache().posision(), ed, db))
                cache.reaset();

            return new NullVariabel();
        }
    }
}
