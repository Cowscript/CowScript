using script.help;
using script.stack;

namespace script.builder
{
    public class Function
    {
        public string Name { get; set; }
        public AgumentStack agument = new AgumentStack();
        public CallInterface call;
    }
}
