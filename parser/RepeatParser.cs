using System;
using script.token;
using script.variabel;

namespace script.parser
{
    //repeat is a new sytrack devopled for CowScript. It look like a functin but missing ;. if th contetext return true it call it agian else it stop
    class RepeatParser : ParserInterface
    {
        public void end()
        {}

        public CVar parse(EnegyData ed, Token token)
        {
            if (token.next().type() != TokenType.LeftBue)
                throw new ScriptError("Missing ( after repeat", token.getCache().posision());

            TokenCache cache = ScopeParser.getScope(token);
            token.next();

            while (new VariabelParser().parse(ed, cache).toBoolean(token.getCache().posision()))
                cache.reaset();

            return new NullVariabel();
        }
    }
}
