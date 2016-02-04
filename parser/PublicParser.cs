using script.token;
using script.variabel;
using System;

namespace script.parser
{
    class PublicParser : ParserInterface
    {
        public void end(EnegyData data, VariabelDatabase db)
        {}

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if(token.next().type() == TokenType.Function)
            {
                functionParser p = new functionParser(true);
                p.parse(ed, db, token);
                p.end(ed, db);
            }else if(token.getCache().type() == TokenType.Class)
            {
                ClassParser p = new ClassParser(true);
                p.parse(ed, db, token);
                p.end(ed, db);
            }
            else
            {
                ed.setError(new ScriptError("unknown token after public: " + token.getCache().ToString(), token.getCache().posision()), db);
            }
            return new NullVariabel();
        }
    }
}
