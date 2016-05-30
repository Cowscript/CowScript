using script.builder;
using script.Type;
using script.variabel;

namespace script.plugin
{
    class Parser : PluginInterface
    {
        public string Name { get { return "parser"; } }

        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Function version = new Function();
            version.Name = "version";
            version.call += Version_call;
            database.pushFunction(version, data);
        }

        private CVar Version_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return Types.toString(Energy.VERSION, data, db, pos);
        }
    }
}
