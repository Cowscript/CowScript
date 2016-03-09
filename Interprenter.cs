using script.parser;
using script.token;
using System.Collections.Generic;

namespace script
{
    public class Interprenter
    {
        private static Dictionary<TokenType, ParserInterface> parsers = null;
        private static PublicParser pp;

        public static void parseFile(Token token, EnegyData data, VariabelDatabase db)
        {
            if (parsers == null)
                setParser();

            token.next();
            while(data.State == RunningState.Normal && token.getCache().type() != TokenType.EOF)
            {
                ParserInterface p;

                switch (token.getCache().type())
                {
                    case TokenType.Use:
                    case TokenType.Class:
                    case TokenType.Function:
                        p = parsers[token.getCache().type()];
                        break;
                    case TokenType.Public:
                        p = pp;
                        break;
                    default:
                        data.setError(new ScriptError("In file you are only allow to use 'use', 'public', 'class' or 'function'", token.getCache().posision()), db);
                        return;
                }

                p.parse(data, db, token);
            }
        }

        public static void parse(Token token, EnegyData data, VariabelDatabase db)
        {
            if(parsers == null)
            {
                setParser();
            }

            token.next();
            TokenType type;
            while (data.State == RunningState.Normal && (type = token.getCache().type()) != TokenType.EOF)
            {
                if (parsers.ContainsKey(type))
                {   
                    parsers[type].parse(data, db, token);
                }
                else
                {
                    new VariabelParser().parse(data, db, token);
                }
            }
        }

        private static void setParser()
        {
            parsers = new Dictionary<TokenType, ParserInterface>();
            parsers.Add(TokenType.If,       new IfParser());
            parsers.Add(TokenType.Function, new functionParser());
            parsers.Add(TokenType.Return,   new ReturnParser());
            parsers.Add(TokenType.Use,      new UseParser());
            parsers.Add(TokenType.Foreach,  new ForeachParser());
            parsers.Add(TokenType.Class,    new ClassParser());
            parsers.Add(TokenType.While,    new WhileParser());
            parsers.Add(TokenType.Repeat,   new RepeatParser());
            parsers.Add(TokenType.For,      new ForParser());
            parsers.Add(TokenType.Break,    new BreakParser());
            parsers.Add(TokenType.Continue, new ContinueParser());
            parsers.Add(TokenType.Unset,    new UnsetParser());

            pp = new PublicParser();
        }
    }
}
