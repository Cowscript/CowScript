using System;
using script.token;
using script.variabel;

namespace script.parser
{
    class UnsetParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {

            UnsetNext(token, ed, db);

            while (token.next().type() == TokenType.Comma)
                UnsetNext(token, ed, db);


            if(token.getCache().type() != TokenType.End)
            {
                ed.setError(new ScriptError("Missing ; in end of unset", token.getCache().posision()), db);
                return null;
            }
            token.next();
            return null;
        }

        private void UnsetNext(Token token, EnegyData data, VariabelDatabase db)
        {
            if(token.next().type() != TokenType.Variabel)
            {
                data.setError(new ScriptError("Missing variabel in 'unset' statmenet", token.getCache().posision()), db);
                return;
            }

            //control wee has the variabel in the variabel database. 
            if (!db.isExists(token.getCache().ToString()))
            {
                data.setError(new ScriptError("Unknown variabel: " + token.getCache().ToString(), token.getCache().posision()), db);
                return;
            }

            //is this variabel a global. 
            if (!db.allowedOveride(token.getCache().ToString()))
            {
                data.setError(new ScriptError("You can not unset the variabel: " + token.getCache().ToString(), token.getCache().posision()), db);
                return;
            }

            //unset the variabel. 
            db.removeVariabel(token.getCache().ToString());
        }
    }
}
