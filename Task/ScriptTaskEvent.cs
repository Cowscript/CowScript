namespace script.Task
{
    public class ScriptTaskEvent
    {
        public delegate bool controlReady();
        public event controlReady state;

        public bool isReady()
        {
            if (state == null)
                return true;//no event is set so it is true :)

            return state();
        }
    }
}
