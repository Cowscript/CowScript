using script.plugin.File;
using script.token;
using script.variabel;
using System.IO;

namespace script.parser
{
    class UseParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            token.next();
            string plugin = new VariabelParser().parse(ed, db, token).toString(token.getCache().posision(), ed, db);

            //control if the plugin exists in the system
            if(ed.Plugin.exists(plugin))
            {
                //wee has the plugin and load it :)
                ed.Plugin.open(db, plugin, ed, token.getCache().posision());
            }
            else
            {
                if(ed.Config.get("file.enabled", "false") == "false")
                {
                    ed.setError(new ScriptError("It is not allow to use file in use. 'file.enabled' is not set.", token.getCache().posision()), db);
                    return new NullVariabel();
                }

                parseFile(ed, db, token.getCache().posision(), plugin);
            }

            return new NullVariabel();
        }

        private void parseFile(EnegyData ed, VariabelDatabase db, Posision pos, string plugin)
        {
            if (!File.Exists(plugin))
            {
                ed.setError(new ScriptError("Unknown file: " + plugin, pos), db);
                return;
            }

            FileEnergy.parse(ed, new FileVariabelDatabase(db), plugin);
        }
    }
}
