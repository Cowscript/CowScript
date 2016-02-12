using script.token;
using script.variabel;

namespace script.parser
{
    class BreakParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            if(token.next().type() != TokenType.End)
            {
                ed.setError(new ScriptError("Missing ; after 'break'", token.getCache().posision()), db);
                return null;
            }

            token.next();

            ed.setBreak();
            return null;
        }
    }
}
