using script.plugin;
using script.variabel;
using System.IO;

namespace script
{
    public class EnegyData
    {
       public static int getAgumentSize(EnegyData data)
        {
            int size;

            if(int.TryParse(data.Config.get("agument.buffer.size", "20"), out size))
            {
                return size;
            }

            return 20;
        }

       public FunctionVariabel ErrorHandler { get; set; }
       public ScriptConfig Config { get; set; }  
       public virtual RunningState State { get; private set; }
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

        public virtual void setError(ScriptError er, VariabelDatabase db)
        {
            if (State == RunningState.Error)
                return;
            State = RunningState.Error;
            ErrorData = er;

            if (ErrorHandler != null)
            {
                if(Config.get("error.log.file", "null") != "null")
                {
                    FileStream fs;

                    //wee should save the error in a file
                    if(File.Exists(Config.get("error.log.file", "")))
                    {
                        fs = File.Create(Config.get("error.log.file", ""));
                    }
                    else
                    {
                        fs = File.OpenWrite(Config.get("error.log.file", ""));
                    }

                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("[Line: " + er.Posision.Line + ", Row: " + er.Posision.Row + "]" + er.Message);
                    sw.Close();
                    fs.Close();
                }

                //okay here may not be a error handler and state must be set to normal
                FunctionVariabel eh = ErrorHandler;
                ErrorHandler = null;
                if (Config.get("error.handler.enable", "1") == "1")
                {
                    State = RunningState.Normal;
                    eh.call(this, db, er.Message, er.Posision.Line, er.Posision.Row);
                }
            }
        }

        public virtual void setBreak()
        {
            State = RunningState.Break;
        }

        public virtual void setContinue()
        {
            State = RunningState.Continue;
        }

        public virtual void setNormal()
        {
            State = RunningState.Normal;
        }
    }

    public enum RunningState
    {
        Normal, 
        Return,
        Error,
        Break, 
        Continue,
    }
}
