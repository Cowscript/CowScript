using script.token;
using script.variabel;

namespace script.parser
{
    class PublicParser : ParserInterface
    {

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() == TokenType.Function)
            {
                functionParser p = new functionParser();
                p.parseFunction(ed, db, token, true, true);
            }else if(token.getCache().type() == TokenType.Class)
            {
                ClassParser p = new ClassParser();
                p.parse(ed, db, token);
            }
            else
            {
                ed.setError(new ScriptError("unknown token after public: " + token.getCache().ToString(), token.getCache().posision()), db);
            }
            return new NullVariabel();
        }
    }
}
