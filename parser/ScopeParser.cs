using script.token;
using System.Collections;

namespace script.parser
{
    class ScopeParser
    {
        public static TokenCache getScope(Token token)
        {
            ArrayList cache = new ArrayList();
            TokenBuffer buffer;

            int happens = 1;

            while((buffer = token.next()).type() != TokenType.EOF)
            {
                if (buffer.type() == TokenType.RightBue)
                {
                    happens--;
                    if (happens == 0)
                        return new TokenCache(cache);

                    cache.Add(buffer);
                    continue;
                }
                else if (buffer.type() == TokenType.LeftBue)
                    happens++;

                cache.Add(buffer);
            }

            throw new ScriptError("Missing ) ", token.getCache().posision());
        }
    }
}
