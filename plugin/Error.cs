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
            error.call += Error_call;

            database.pushFunction(error);

            Function errorCallback = new Function();
            errorCallback.Name = "errorCallback";
            errorCallback.agument.push("function", "f");//here wee say wee want a agument there is function and
            errorCallback.call += ErrorCallback_call; ;
            database.pushFunction(errorCallback);
        }

        private CVar ErrorCallback_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            data.ErrorHandler = (FunctionVariabel)stack[0];
            return new BooleanVariabel(true);
        }

        private CVar Error_call(CVar[] stack, VariabelDatabase db, EnegyData data, Posision pos)
        {
            throw new ScriptError(stack[0].toString(pos, data, db), pos);
        }
    }
}
