using script.token;
using script.variabel;

namespace script.parser
{
    interface ParserInterface
    {
        CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile);
    }
}
