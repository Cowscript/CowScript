namespace script.token
{
    public class TokenBuffer
    {

        private TokenType t;
        private string context;
        private Posision pos;

        public TokenBuffer(string context, TokenType t, Posision p)
        {
            this.context = context;
            this.t = t;
            pos = p;
        }

        public override string ToString()
        {
            return context;
        }

        public Posision posision()
        {
            return pos;
        }

        public TokenType type()
        {
            return t;
        }
    }
}
