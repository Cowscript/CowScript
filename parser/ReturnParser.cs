using script.token;
using script.variabel;

namespace script.parser
{
    class ReturnParser : ParserInterface
    {

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            if (token.next().type() != TokenType.End)
            {
                ed.setReturn(new VariabelParser().parse(ed, db, token, isFile));
            }
            else {
                token.next();
                ed.setReturn(new NullVariabel());
            }

            return null;
        }
    }
}
