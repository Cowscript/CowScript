using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class IfParser : ParserInterface
    {
        private Token token;
        private EnegyData ed;

        public void end()
        {
        }

        public CVar parse(EnegyData ed, Token token)
        {
            this.token = token;
            this.ed = ed;
            begin(false);
            return null;
        }

        private void begin(bool befor) { 
            //if token ( now ?
            if(token.next().type() != TokenType.LeftBue)
            {
                throw new ScriptError("Missing ( after if", token.getCache().posision());
            }

            if(token.next().type() == TokenType.RightBue)
            {
                //it is a empty scope :)
                throw new ScriptError("You can not use a if whit empty scope", token.getCache().posision());
            }

            //get context :)
            CVar scope = new VariabelParser().parse(ed, token);

            //control if wee got a ) 
            if(token.getCache().type() != TokenType.RightBue)
            {
                //wee missing ) 
                throw new ScriptError("Missing ) got: " + token.getCache().ToString(), token.getCache().posision());
            }

            ArrayList body = BodyParser.parse(token);
            token.next();

            if(!befor && scope.toBoolean(token.getCache().posision()))
            {
                ed.Interprenter.parse(new TokenCache(body));
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
            ArrayList body = BodyParser.parse(token);
            token.next();
            if (!befor)
                ed.Interprenter.parse(new TokenCache(body));
        }
    }
}
