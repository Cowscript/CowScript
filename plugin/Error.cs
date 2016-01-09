using script.builder;
using script.help;
using script.stack;
using script.variabel;

namespace script.plugin
{
    class Error : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Function error = new Function();
            error.Name = "error";
            error.agument.push("string", "message");//here wee say wee want a agument there is string and name string :)
            error.call = new errorHelp();

            database.pushFunction(error);

            Function errorCallback = new Function();
            errorCallback.Name = "errorCallback";
            errorCallback.agument.push("function", "f");//here wee say wee want a agument there is function and
            errorCallback.call = new errorCallbackHelp();
            database.pushFunction(errorCallback);
        }
    }

    public class errorHelp : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            throw new ScriptError(stack.pop().toString(new Posision(0, 0)), new Posision(0, 0));
        }
    }

    public class errorCallbackHelp : CallInterface
    {
        public CVar call(CallAgumentStack stack, EnegyData data)
        {
            if (data.Befor == null)
                return new BooleanVariabel(false);

            data.Befor.Error = (FunctionVariabel)stack.pop();

            return new BooleanVariabel(true);
        }
    }
}
