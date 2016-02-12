using System;
using script.token;
using script.variabel;

namespace script.parser
{
    class ContinueParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            if(token.next().type() != TokenType.End)
            {
                ed.setError(new ScriptError("Missing ; in end of 'continue'", token.getCache().posision()), db);
                return null;
            }

            token.next();
            ed.setContinue();
            return null;
        }
    }
}
