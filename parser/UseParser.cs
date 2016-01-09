using script.token;
using script.variabel;

namespace script.parser
{
    class UseParser : ParserInterface
    {
        private VariabelParser p = new VariabelParser();

        public void end()
        {
            p.end();
        }

        public CVar parse(EnegyData ed, Token token)
        {
            token.next();
            CVar use = p.parse(ed, token);
            if (!ed.Plugin.exists(use.toString(token.getCache().posision())))
                throw new ScriptError("Unknown plugin: " + use.toString(token.getCache().posision()), token.getCache().posision());

            ed.Plugin.open(ed.VariabelDatabase, use.toString(token.getCache().posision()));
            return null;
        }
    }
}
