using script.token;
using script.variabel;

namespace script.parser
{
    class ReturnParser : ParserInterface
    {
        private VariabelParser p = new VariabelParser();

        public void end()
        {
            
        }

        public CVar parse(EnegyData ed, Token token)
        {
            if (token.next().type() != TokenType.End)
            {
                ed.Return = p.parse(ed, token);
                p.end();
            }
            else {
                token.next();
                ed.Return = new NullVariabel();
            }

            return ed.Return;
        }
    }
}
