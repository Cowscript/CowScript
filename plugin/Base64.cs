using script.builder;
using script.stack;
using script.Type;
using script.variabel;
using System;
using System.Text;

namespace script.plugin
{
    class Base64 : PluginInterface
    {
        public void open(VariabelDatabase database, EnegyData data, Posision pos)
        {
            Class base64 = new Class("Base64");

            Method encode = new Method("encode");
            encode.SetStatic();
            encode.GetAgumentStack().push("string", "text");
            encode.SetBody(Encode_caller);
            base64.SetMethod(encode, data, database, pos);

            Method decode = new Method("decode");
            decode.SetStatic();
            decode.GetAgumentStack().push("string", "text");
            decode.SetBody(Decode_caller);
            base64.SetMethod(decode, data, database, pos);

            database.pushClass(base64, data);
        }

        private CVar Decode_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            byte[] buffer = Convert.FromBase64String(stack[0].toString(pos, data, db));
            return Types.toString(Encoding.UTF8.GetString(buffer), data, db, pos);
        }

        private CVar Encode_caller(CVar c, VariabelDatabase db, CVar[] stack, EnegyData data, Posision pos)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(stack[0].toString(pos, data, db));
            return Types.toString(Convert.ToBase64String(buffer), data, db, pos);
        }
    }
}
