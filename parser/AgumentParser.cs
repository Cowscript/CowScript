using script.parser;
using script.token;
using script.variabel;

namespace script.stack
{
    class AgumentParser
    {
        public static AgumentStack parseAguments(Token token, VariabelDatabase database, Interprenter interprenter, FunctionVariabel error)
        {
            return parseAguments(token, new AgumentStack(), database, interprenter, error);
        }

        public static AgumentStack parseAguments(Token token, AgumentStack agument, VariabelDatabase database, Interprenter interprenter, FunctionVariabel error)
        {
            if(token.next().type() != TokenType.LeftBue)
            {
                throw new ScriptError("Excpect ( got: " + token.getCache().ToString(), token.getCache().posision());
            }

            //control if wee need to look and parse aguments :)
            if(token.next().type() != TokenType.RightBue)
            {
                //wee need :)
                getSingleAguments(token, agument, database, interprenter, error);
                while(token.getCache().type() == TokenType.Comma)
                {
                    token.next();
                    getSingleAguments(token, agument, database, interprenter, error);
                }
            }

            //control wee got to ) 
            if(token.getCache().type() != TokenType.RightBue)
            {
                throw new ScriptError("Missing ) after function aguments got: " + token.getCache().ToString(), token.getCache().posision());
            }

            return agument;
        }

        private static void getSingleAguments(Token token, AgumentStack agument, VariabelDatabase database, Interprenter interprenter, FunctionVariabel error)
        {   string type = null;
            string name = null;
            CVar value = null;

            //okay wee got a variabel. control if it is a type :)
            if (database.isType(token.getCache().ToString()))
            {
                //yes it is a type :)
                type = token.getCache().ToString();

                //okay let try to find the name 
                if(token.next().type() != TokenType.Variabel)
                {
                    throw new ScriptError("After type there must be a type", token.getCache().posision());
                }

                name = token.getCache().ToString();
            }
            else
            {
                if (token.getCache().type() != TokenType.Variabel)
                    throw new ScriptError("Unknown agument token: " + token.getCache().ToString(), token.getCache().posision());
                name = token.getCache().ToString();
            }

            if (token.next().type() == TokenType.Assigen)
            {
                token.next();
                VariabelParser parser = new VariabelParser();
                value = parser.parse(new EnegyData(new VariabelDatabase(), interprenter, null, error, null), token);
            }

            agument.push(type, name, value);
        }
    }
}
