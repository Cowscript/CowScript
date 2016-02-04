using System.Collections;

namespace script.token
{
    public class TokenCache : Token
    {
        private ArrayList cache = new ArrayList();
        private int id = -1;

        public TokenCache(ArrayList c, EnegyData data, VariabelDatabase vd) : base(data, vd)
        {
            cache = c;
        }

        public override TokenBuffer getCache()
        {
            if (id == -1)
                return next();

            if (id >= cache.Count)
                return new TokenBuffer("End Of file", TokenType.EOF, new Posision(0, 0));

            return (TokenBuffer)cache[id];
        }

        public override TokenBuffer next()
        {
            id++;

            if (id >= cache.Count)
                return new TokenBuffer("End Of file", TokenType.EOF, new Posision(0, 0));

            return (TokenBuffer)cache[id];
        }
    }
}
