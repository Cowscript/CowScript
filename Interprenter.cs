using script.parser;
using script.token;
using script.variabel;
using System.Collections.Generic;

namespace script
{
    public class Interprenter
    {
        public ObjectVariabel ObjectVariabel { private set; get; }
        public ClassVariabel StaticVariabel { private set; get; }

        private static Dictionary<TokenType, ParserInterface> parsers = null;

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
            if(parsers == null)
            {
                setParser();
            }

            token.next();
            //run thrue the script (if Run is true and return is null)
            while (data.State == RunningState.Normal && token.getCache().type() != TokenType.EOF)
            {
                if (parsers.ContainsKey(token.getCache().type()))
                {
                    parsers[token.getCache().type()].parse(data, db, token, isFile);
                }
                else
                {
                    new VariabelParser().parse(data, db, token, isFile);
                }
            }
        }

        private static void setParser()
        {
            parsers = new Dictionary<TokenType, ParserInterface>();
            parsers.Add(TokenType.If, new IfParser());
            parsers.Add(TokenType.Function, new functionParser());
            parsers.Add(TokenType.Return, new ReturnParser());
            parsers.Add(TokenType.Use, new UseParser());
            parsers.Add(TokenType.Foreach, new ForeachParser());
            parsers.Add(TokenType.Class, new ClassParser());
            parsers.Add(TokenType.While, new WhileParser());
            parsers.Add(TokenType.Repeat, new RepeatParser());
            parsers.Add(TokenType.For, new ForParser());
            parsers.Add(TokenType.Public, new PublicParser());
            parsers.Add(TokenType.Break, new BreakParser());
            parsers.Add(TokenType.Continue, new ContinueParser());
            parsers.Add(TokenType.Unset, new UnsetParser());
        }
    }
}
