using script.stack;
using script.variabel;

namespace script.help
{
    public interface CallInterface
    {
        CVar call(CallAgumentStack stack, EnegyData data);
    }
}
