using script.parser;
using script.plugin;
using script.token;
using script.variabel;
using System;

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
            parse(token, data, db, false);
        }

        public static void parse(Token token, EnegyData data, VariabelDatabase db, bool isFile)
        {
            token.next();
            ParserInterface pi;
            //run thrue the script (if Run is true and return is null)
            while (data.State == RunningState.Normal && token.getCache().type() != TokenType.EOF && (pi = getParser(token, isFile, data, db)) != null)
            {
                pi.parse(data, db, token);
                pi.end(data, db);//in some parser it will have this to detext if end is okay (like variabel parseren)
            }
        }

        private static ParserInterface getParser(Token token, bool isFile, EnegyData data, VariabelDatabase database)
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
                case TokenType.For:
                    return new ForParser();
                case TokenType.Public:
                    if (!isFile)
                    {
                        data.setError(new ScriptError("public [function/class] can only be uses when you parse file from use 'path'", token.getCache().posision()), database);
                        return null;
                    }

                    return new PublicParser();
                default:
                    return new VariabelParser();
            }
        }
    }
}
