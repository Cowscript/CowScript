using script.plugin;
using script.variabel;

namespace script
{
    public class EnegyData
    {
       public FunctionVariabel ErrorHandler { get; set; }
       public ScriptConfig Config { get; set; }  
       public RunningState State { get; private set; }
       public PluginContainer Plugin { get; set; }

       public ScriptError ErrorData { get; private set; }
       private CVar ReturnContext { get; set; }

        public EnegyData()
        {
            Plugin = new PluginContainer();
        }

        public CVar getReturn()
        {
            if (State != RunningState.Return)
                return new NullVariabel();

            State = RunningState.Normal;
            return ReturnContext;
        }

        public void setReturn(CVar r)
        {
            State = RunningState.Return;
            ReturnContext = r;
        }

        public void setError(ScriptError er, VariabelDatabase db)
        {
            State = RunningState.Error;
            ErrorData = er;
            if(ErrorHandler != null)
            {
                ErrorHandler.call(this, db, er.Message, er.Posision.Line, er.Posision.Row);
            }
        }
    }

    public enum RunningState
    {
        Normal, 
        Return,
        Error
    }
}
