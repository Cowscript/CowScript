using script.variabel;
using System.Collections;

namespace script.stack
{
    public class AgumentStack
    {
        private ArrayList container = new ArrayList();

        public void push(string type, string name, CVar value)
        {
            container.Add(new AgumentStackData()
            {
                Type = type,
                Name = name,
                Value = value,
            });
        }

        public void push(string name)
        {
            push(null, name, null);
        }

        public void push(string type, string name)
        {
            push(type, name, null);
        }

        public AgumentStackData get(int i)
        {
            return (AgumentStackData)container[i];
        }

        public int size()
        {
            return container.Count;
        }
    }

    public class AgumentStackData
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public CVar Value { get; set; }

        public bool hasType()
        {
            return Type != null;
        }

        public bool hasValue()
        {
            return Value != null;
        }
    }
}
