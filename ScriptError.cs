using script.token;
using System;

namespace script
{
    public class ScriptError : Exception
    {
        public Posision Posision { private set; get; }
        public ScriptError(string msg, TokenPosision pos) : base(msg)
        {
            Posision = pos.toPosision();
        }

        public ScriptError(string msg, Posision pos) : base(msg)
        {
            Posision = pos;
        }
    }
}
