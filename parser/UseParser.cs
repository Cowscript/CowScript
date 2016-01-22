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

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            token.next();
            CVar use = p.parse(ed, db, token);
            if (!ed.Plugin.exists(use.toString(token.getCache().posision(), ed, db)))
            {
                ed.setError(new ScriptError("Unknown plugin: " + use.toString(token.getCache().posision(), ed, db), token.getCache().posision()), db);
                return new NullVariabel();
            }

            ed.Plugin.open(db, use.toString(token.getCache().posision(), ed, db));
            return null;
        }
    }
}
