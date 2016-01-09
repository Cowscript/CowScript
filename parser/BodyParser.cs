using script.token;
using System.Collections;

namespace script.parser
{
    class BodyParser
    {
        public static ArrayList parse(Token token)
        {
            ArrayList cache = new ArrayList();
            //wee control the next token to see if it {
            if(token.next().type() == TokenType.LeftTuborg)
            {
                int happens = 1;
                TokenBuffer buffer;
                while((buffer = token.next()).type() != TokenType.EOF)
                {
                    if (buffer.type() == TokenType.RightTuborg)
                    {
                        happens--;
                        if (happens == 0)
                            break;
                    }
                    else if (buffer.type() == TokenType.LeftTuborg)
                        happens++;

                    cache.Add(buffer);
                }

                if(token.getCache().type() != TokenType.RightTuborg)
                {
                    throw new ScriptError("Missing } got " + token.getCache().ToString(), token.getCache().posision());
                }
            }

            return cache;
        }
    }
}
