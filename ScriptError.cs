using script.token;
using System;

namespace script
{
    public class ScriptError
    {
        public Posision Posision { private set; get; }
        public string Message { get; private set; }
        public ScriptError(string msg, TokenPosision pos)
        {
            Posision = pos.toPosision();
            Message = msg;
        }

        public ScriptError(string msg, Posision pos)
        {
            Posision = pos;
            Message = msg;
        }
    }
}
