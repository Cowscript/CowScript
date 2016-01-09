using script.builder;
using script.plugin;
using script.token;
using System.IO;

namespace script
{
    public class Energy
    {

        public VariabelDatabase VariabelDatabase { set; get; }
        public PluginContainer plugin = new PluginContainer();

        public Energy()
        {
            VariabelDatabase = new VariabelDatabase();
        }

        public void parse(string script)
        {
            parse(new StringReader(script));
        }

        public void parse(StringReader reader)
        {
            parse((TextReader)reader);
        }

        public void parse(TextReader reader)
        {
            Interprenter.parse(VariabelDatabase, plugin, null).parse(new Token(reader));
        }

        public void push(Function func)
        {
            VariabelDatabase.pushFunction(func);
        }

        public void push(Class c)
        {
            VariabelDatabase.pushClass(c);
        }
    }
}
