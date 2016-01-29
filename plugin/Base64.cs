using script.builder;
using script.stack;
using script.variabel;
using System;
using System.Text;

namespace script.plugin
{
    class Base64 : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data)
        {
            Class base64 = new Class("Base64");

            ClassStaticMethods encode = new ClassStaticMethods(base64, "encode");
            encode.Aguments.push("string", "text");
            encode.caller += Encode_caller;
            encode.create();

            ClassStaticMethods decode = new ClassStaticMethods(base64, "decode");
            decode.Aguments.push("string", "text");
            decode.caller += Decode_caller;
            decode.create();

            database.pushClass(base64, data);
        }

        private CVar Decode_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            byte[] buffer = Convert.FromBase64String(stack[0].toString(pos, data, db));
            return StringVariabel.CreateString(data, db, pos, Encoding.UTF8.GetString(buffer));
        }

        private CVar Encode_caller(ClassVariabel c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(stack[0].toString(pos, data, db));
            return StringVariabel.CreateString(data, db, pos, Convert.ToBase64String(buffer));
        }
    }
}
