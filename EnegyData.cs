using script.plugin;
using script.variabel;

namespace script
{
    public class EnegyData
    {
        public bool Run { set; get; }
        public CVar Return { set; get; }
        public VariabelDatabase VariabelDatabase { set; get; }
        public Interprenter Interprenter { set; get; }
        public PluginContainer Plugin { set; get; }
        public FunctionVariabel Error { set; get; }
        public EnegyData Befor { set; get; }

        public EnegyData(VariabelDatabase databse, Interprenter interprenter, PluginContainer plugin, FunctionVariabel error, EnegyData befor)
        {
            Run = true;
            Return = null;
            Interprenter = interprenter;
            VariabelDatabase = databse;
            Plugin = plugin;
            Error = error;
            Befor = befor;
        }

        public void removeError()
        {
            Error = null;
            if (Befor != null)
                Befor.removeError();
        }
    }
}
