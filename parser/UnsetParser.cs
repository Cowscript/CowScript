using System;
using script.token;
using script.variabel;

namespace script.parser
{
    class UnsetParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if(token.next().type() != TokenType.Variabel)
            {
                ed.setError(new ScriptError("Missing variabel after 'unset'", token.getCache().posision()), db);
                return null;
            }

            string varName = token.getCache().ToString();

            if (!db.isExists(varName))
            {
                ed.setError(new ScriptError("Unknown variabel: " + varName, token.getCache().posision()), db);
                return null;
            }

            if (!db.allowedOveride(varName))
            {
                ed.setError(new ScriptError("You can not unset: " + varName, token.getCache().posision()), db);
                return null;
            }

            if(token.next().type() != TokenType.End)
            {
                ed.setError(new ScriptError("Missing ; in end of unset", token.getCache().posision()), db);
                return null;
            }

            db.removeVariabel(varName);
            token.next();
            return null;
        }
    }
}
