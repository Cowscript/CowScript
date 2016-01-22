using script.parser;
using script.token;
using script.variabel;

namespace script.stack
{
    class AgumentParser
    {

        public static AgumentStack parseAguments(Token token, VariabelDatabase database, EnegyData data)
        {
            AgumentStack agument = new AgumentStack();
            if(token.next().type() != TokenType.LeftBue)
            {
                data.setError(new ScriptError("Excpect ( got: " + token.getCache().ToString(), token.getCache().posision()), database);
                return agument;
            }

            //control if wee need to look and parse aguments :)
            if(token.next().type() != TokenType.RightBue)
            {
                //wee need :)
                if (!getSingleAguments(token, agument, database, data))
                    return new AgumentStack();
                while(token.getCache().type() == TokenType.Comma)
                {
                    token.next();
                    if (!getSingleAguments(token, agument, database, data))
                        return new AgumentStack();
                }
            }

            //control wee got to ) 
            if(token.getCache().type() != TokenType.RightBue)
            {
                data.setError(new ScriptError("Missing ) after function aguments got: " + token.getCache().ToString(), token.getCache().posision()), database);
            }

            return agument;
        }

        private static bool getSingleAguments(Token token, AgumentStack agument, VariabelDatabase database, EnegyData data)
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
                    data.setError(new ScriptError("After type there must be a type", token.getCache().posision()), database);
                    return false;
                }

                name = token.getCache().ToString();
            }
            else
            {
                if (token.getCache().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("Unknown agument token: " + token.getCache().ToString(), token.getCache().posision()), database);
                    return false;
                }
                name = token.getCache().ToString();
            }

            if (token.next().type() == TokenType.Assigen)
            {
                token.next();
                VariabelParser parser = new VariabelParser();
                value = parser.parse(data, database, token);
            }

            agument.push(type, name, value);
            return true;
        }
    }
}
