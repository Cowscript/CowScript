using script.parser;
using script.plugin;
using script.token;
using script.variabel;

namespace script
{
    public class Interprenter
    {
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

        public static void parse(Token token, EnegyData data, VariabelDatabase db) {
            token.next();
            //run thrue the script (if Run is true and return is null)
            while (data.State == RunningState.Normal && token.getCache().type() != TokenType.EOF)
            {
                ParserInterface pi = getParser(token);
                pi.parse(data, db, token);
                pi.end();//in some parser it will have this to detext if end is okay (like variabel parseren)
            }
        }

        private static ParserInterface getParser(Token token)
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
