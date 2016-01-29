using script.token;
using script.variabel;
using System.Text.RegularExpressions;

namespace script.parser
{
    class UseParser : ParserInterface
    {
        private VariabelParser p = new VariabelParser();

        public void end(EnegyData ed, VariabelDatabase db)
        {
            p.end(ed, db);
        }

        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            token.next();
            string plugin = p.parse(ed, db, token).toString(token.getCache().posision(), ed, db);
            
            //control if the plugin exists in the system
            if(ed.Plugin.exists(plugin))
            {
                //wee has the plugin and load it :)
                ed.Plugin.open(db, plugin, ed);
            }
            else
            {
                if(ed.Config.get("file.base", "0") == "0")
                {
                    ed.setError(new ScriptError("It is not allow to use file in use. 'file.base' is not set.", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                parseFile(ed, db, token.getCache().posision());
            }

            return new NullVariabel();
        }

        private void parseFile(EnegyData ed, VariabelDatabase db, Posision pos)
        {

        }
    }
}
