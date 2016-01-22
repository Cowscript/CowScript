namespace script.Task
{
    public class ScriptEndEvent
    {
        public delegate void ie();
        public event ie inEnd; 

        public void callInEnd()
        {
            if (inEnd != null)
                inEnd();
        }
    }
}
