using script.builder;
using script.variabel;

namespace script.plugin
{
    class Parser : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Function version = new Function();
            version.Name = "version";
            version.call += Version_call;
            database.pushFunction(version, data);
        }

        private CVar Version_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            return StringVariabel.CreateString(data, db, pos, Energy.VERSION);
        }
    }
}
