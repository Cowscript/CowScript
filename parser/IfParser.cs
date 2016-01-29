using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class IfParser : ParserInterface
    {
        private Token token;
        private EnegyData ed;
        private VariabelDatabase db;

        public void end(EnegyData data, VariabelDatabase db)
        {
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            this.token = token;
            this.ed = ed;
            this.db = db;
            begin(false);
            return null;
        }

        private void begin(bool befor) { 
            //if token ( now ?
            if(token.next().type() != TokenType.LeftBue)
            {
                ed.setError(new ScriptError("Missing ( after if", token.getCache().posision()), db);
                return;
            }

            if(token.next().type() == TokenType.RightBue)
            {
                //it is a empty scope :)
                ed.setError(new ScriptError("You can not use a if whit empty scope", token.getCache().posision()), db);
                return;
            }

            //get context :)
            CVar scope = new VariabelParser().parse(ed, db, token);

            //control if wee got a ) 
            if(token.getCache().type() != TokenType.RightBue)
            {
                //wee missing ) 
                ed.setError(new ScriptError("Missing ) got: " + token.getCache().ToString(), token.getCache().posision()), db);
                return;
            }

            ArrayList body = BodyParser.parse(token, ed, db);
            token.next();

            if(!befor && scope.toBoolean(token.getCache().posision(), ed, db))
            {
                Interprenter.parse(new TokenCache(body, ed, db), ed, db);
                befor = true;
            }

            //control if next is elseif :)
            if(token.getCache().type() == TokenType.Elseif)
            {
                begin(befor);
            }else if(token.getCache().type() == TokenType.Else)
            {
                doElse(befor);
            }
        }

        private void doElse(bool befor)
        {
            ArrayList body = BodyParser.parse(token, ed, db);
            token.next();
            if (!befor)
                Interprenter.parse(new TokenCache(body, ed, db), ed, db);
        }
    }
}
