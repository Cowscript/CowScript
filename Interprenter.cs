using script.parser;
using script.plugin;
using script.token;
using script.variabel;

namespace script
{
    public class Interprenter
    {
        public static Interprenter parse(VariabelDatabase database, PluginContainer plugin, EnegyData befor)
        {
            //set level 1
            Interprenter i = new Interprenter();
            EnegyData e = new EnegyData(database, i, plugin, befor == null ? null : befor.Error, befor);
            i.data = e;
            return i;
        }
        
        public EnegyData data;
        public ObjectVariabel ObjectVariabel { private set; get; }
        public ClassVariabel StaticVariabel { private set; get; }

        public void setObject(ObjectVariabel objectVariabel)
        {
            ObjectVariabel = objectVariabel;
        }

        public void setStatic(ClassVariabel c)
        {
            StaticVariabel = c;
        }

        public CVar parse(Token token) {
            try
            {
                return beginParse(token);
            }catch(ScriptError e)
            {
                if(data.Error != null)
                {
                    FunctionVariabel error = data.Error;
                    data.removeError();
                    error.call(data, e.Message, e.Posision.Line, e.Posision.Row);
                    return new NullVariabel();
                }
                else
                {
                    throw e;
                }
            }
        }

        private CVar beginParse(Token token) {
            token.next();
            //run thrue the script (if Run is true and return is null)
            while (data.Run && data.Return == null && token.getCache().type() != TokenType.EOF)
            {
                ParserInterface pi = getParser(token);
                pi.parse(data, token);
                pi.end();//in some parser it will have this to detext if end is okay (like variabel parseren)
            }

            if (data.Return != null)
                return data.Return;

            return null;
        }

        private ParserInterface getParser(Token token)
        {
            switch (token.getCache().type())
            {
                case TokenType.If:
                    return new IfParser();
                case TokenType.Function:
                    return new functionParser();
                case TokenType.Return:
                    return new ReturnParser();
                case TokenType.Use:
                    return new UseParser();
                case TokenType.Foreach:
                    return new ForeachParser();
                case TokenType.Class:
                    return new ClassParser();
                case TokenType.While:
                    return new WhileParser();
                case TokenType.Repeat:
                    return new RepeatParser();
                default:
                    return new VariabelParser();
            }
        }
    }
}
