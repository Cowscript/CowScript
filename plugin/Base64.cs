using script.builder;
using script.stack;
using script.variabel;
using System;
using System.Text;

namespace script.plugin
{
    class Base64 : PluginInterface
    {
        public void open(VariabelDatabase database)
        {
            Class base64 = new Class("Base64");

            ClassStaticMethods encode = new ClassStaticMethods(base64);
            encode.setName("encode");
            encode.setAccess(true);
            AgumentStack encodeStack = new AgumentStack();
            encodeStack.push("string", "text");
            encode.Aguments = encodeStack;
            encode.caller += Encode_caller;
            encode.create();

            ClassStaticMethods decode = new ClassStaticMethods(base64);
            decode.setName("decode");
            decode.setAccess(true);
            AgumentStack decodeStack = new AgumentStack();
            decodeStack.push("string", "text");
            decode.Aguments = decodeStack;
            decode.caller += Decode_caller;
            decode.create();
        }

        private CVar Decode_caller(ClassVariabel c, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            byte[] buffer = Convert.FromBase64String(stack.pop().toString(new Posision(0,0)));
            return new StringVariabel(Encoding.UTF8.GetString(buffer));
        }

        private CVar Encode_caller(ClassVariabel c, VariabelDatabase db, CallAgumentStack stack, EnegyData data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(stack.pop().toString(new Posision(0, 0)));
            return new StringVariabel(Convert.ToBase64String(buffer));
        }
    }
}
