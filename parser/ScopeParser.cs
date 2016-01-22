using script.token;
using System.Collections;

namespace script.parser
{
    class ScopeParser
    {
        public static TokenCache getScope(Token token, EnegyData data, VariabelDatabase db)
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

            data.setError(new ScriptError("Missing ) ", token.getCache().posision()), db);
            return new TokenCache(new ArrayList());
        }
    }
}
