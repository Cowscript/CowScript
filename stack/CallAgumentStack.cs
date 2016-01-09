using script.variabel;
using System.Collections;

namespace script.stack
{
    public class CallAgumentStack
    {
        private ArrayList aguments = new ArrayList();

        public void push(CVar agument)
        {
            aguments.Add(agument);
        }

        public int size()
        {
            return aguments.Count;
        }

        public CVar pop()
        {
            if (size() == 0)
                return new NullVariabel();

            CVar v = (CVar)aguments[0];
            aguments.Remove(v);
            return v;
        }
    }
}
