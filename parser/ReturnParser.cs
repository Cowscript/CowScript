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

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            if (token.next().type() != TokenType.End)
            {
                ed.setReturn(p.parse(ed, db, token));
                p.end();
            }
            else {
                token.next();
                ed.setReturn(new NullVariabel());
            }

            return null;
        }
    }
}
