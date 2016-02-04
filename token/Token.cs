using System;
using System.Collections;
using System.IO;
using System.Text;

namespace script.token
{
    public class Token
    {

        private TextReader reader;
        private TokenPosision pos = new TokenPosision();
        private TokenBuffer cache = null;
        private ArrayList reservedVariabelName = new ArrayList();
        private Posision bp;

        protected EnegyData data;
        protected VariabelDatabase database;

        public Token()
        {
            reservedVariabelName.Add("extends");
            reservedVariabelName.Add("instanceof");
            reservedVariabelName.Add("is");
            reservedVariabelName.Add("func");
        }

        public Posision getPosision()
        {
            return bp;
        }

        /**
        *Detect if it is a part of variabel :)
        */
        public static bool isVariabelChar(int c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        public static bool isInt(int c)
        {
            return c >= '0' && c <= '9';
        }

        public Token(EnegyData data, VariabelDatabase db) {
            this.data = data;
            database = db;
        }

        public Token(TextReader reader, EnegyData data, VariabelDatabase db)
        {
            this.reader   = reader;
            this.data     = data;
            database      = db;

        }

        public virtual TokenBuffer next()
        {
            return cache = getNextToken();
        }

        public virtual TokenBuffer getCache()
        {
            if (cache == null)
                return next();

            return cache;
        }

        private TokenBuffer getNextToken()
        {
            int c = pop();

            if(c == -1)
            {
                //end of file :)
                return new TokenBuffer("End of file", TokenType.EOF, this.bp);
            }

            switch (c)
            {
                case '{':
                    return new TokenBuffer("{", TokenType.LeftTuborg, bp);
                case '}':
                    return new TokenBuffer("}", TokenType.RightTuborg, bp);
                case ':':
                    return new TokenBuffer(":", TokenType.DublePunk, bp);
                case ',':
                    return new TokenBuffer(",", TokenType.Comma, bp);
                case '[':
                    return new TokenBuffer("[", TokenType.LeftBoks, bp);
                case ']':
                    return new TokenBuffer("]", TokenType.RightBoks, bp);
                case '(':
                    return new TokenBuffer("(", TokenType.LeftBue, bp);
                case ')':
                    return new TokenBuffer(")", TokenType.RightBue, bp);
                case ';':
                    return new TokenBuffer(";", TokenType.End, bp);
                case '#':
                    while (peek() != '\n') pop(false);
                    return getNextToken();
                case '\'':
                case '"':
                    return toString(c);
                case '*':
                    return new TokenBuffer("*", TokenType.Gange, bp);
                case '-':
                    if(peek() == '-')
                    {
                        pop();
                        return new TokenBuffer("--", TokenType.MinusOne, bp);
                    }else if(peek() == '>')
                    {
                        pop();
                        return new TokenBuffer("->", TokenType.ObjectBind, bp);
                    }

                    return new TokenBuffer("-", TokenType.Minus, bp);
                case '+':
                    if(peek() == '+')
                    {
                        pop();
                        return new TokenBuffer("++", TokenType.PlusOne, bp);
                    }

                    return new TokenBuffer("+", TokenType.Plus, bp);
                case '>':
                    if(peek() == '=')
                    {
                        pop();
                        return new TokenBuffer(">=", TokenType.LessEquels, bp);
                    }

                    return new TokenBuffer(">", TokenType.Less, bp);
                case '<':
                    if(peek() == '=')
                    {
                        pop();
                        return new TokenBuffer("<=", TokenType.GreaterEquels, bp);
                    }
                    return new TokenBuffer("<", TokenType.Greater, bp);
                case '!':
                    if(peek() == '=')
                    {
                        pop();
                        return new TokenBuffer("!=", TokenType.NotEquels, bp);
                    }
                    return new TokenBuffer("!", TokenType.Not, bp);
                case '=':
                    if(peek() == '=')
                    {
                        pop();
                        return new TokenBuffer("==", TokenType.Equels, bp);
                    }

                    return new TokenBuffer("=", TokenType.Assigen, bp);
                case '|':
                    if (peek() == '|')
                    {
                        pop();
                        return new TokenBuffer("||", TokenType.Or, bp);
                    }

                    return new TokenBuffer("|", TokenType.Skil, bp);
                case '&':
                    if(peek() == '&')
                    {
                        pop();
                        return new TokenBuffer("&&", TokenType.And, bp);
                    }

                    return new TokenBuffer("&", TokenType.Also, bp);
                case '/':
                    if(peek() == '/')
                    {
                        while (pop(false) != '\n') ;
                        return getNextToken();
                    }else if(peek() == '*')
                    {
                        int cache = pop(false);
                        while(cache != -1)
                        {
                            if(cache == '*' && pop(false) == '/')
                            {
                                break;
                            }

                            cache = pop(false);
                        }

                        if (cache == -1) {
                            return error("Missing */ got end of line", bp);
                        }
                        return getNextToken();
                    }

                    return new TokenBuffer("/", TokenType.Divider, bp);
                default:
                    if (isInt(c))
                    {
                        return new TokenBuffer(toInt(c), TokenType.Int, bp);
                    }
                    else if (isVariabelChar(c))
                    {
                        return getVariabel(c);
                    }
                    break;

            }

            return error("Unknown char: " + (char)c, bp);
        }

        private TokenBuffer getVariabel(int prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((char)prefix);

            int c;

            while (isVariabelChar((c = this.peek())) || isInt(c))
                builder.Append((char)this.pop());

            string name = builder.ToString();

            if (reservedVariabelName.Contains(name))
                return error(name + " is resevd and there for you are not allow to use it", pos.toPosision());

            if (name == "if") {
                return new TokenBuffer("if", TokenType.If, pos.toPosision());
            } else if (name == "elseif") {
                return new TokenBuffer("elseif", TokenType.Elseif, pos.toPosision());
            } else if(name == "else") {
                return new TokenBuffer("else", TokenType.Else, pos.toPosision());
            }else if(name == "true" || name == "false")
            {
                return new TokenBuffer(name, TokenType.Bool, pos.toPosision());
            }else if(name == "null")
            {
                return new TokenBuffer(name, TokenType.Null, pos.toPosision());
            }else if(name == "class")
            {
                return new TokenBuffer(name, TokenType.Class, pos.toPosision());
            }else if(name == "public")
            {
                return new TokenBuffer(name, TokenType.Public, pos.toPosision());
            }else if(name == "private")
            {
                return new TokenBuffer(name, TokenType.Private, pos.toPosision());
            }else if(name == "static")
            {
                return new TokenBuffer(name, TokenType.Static, pos.toPosision());
            }else if(name == "function")
            {
                return new TokenBuffer(name, TokenType.Function, pos.toPosision());
            }else if(name == "return")
            {
                return new TokenBuffer(name, TokenType.Return, pos.toPosision());
            }else if(name == "use")
            {
                return new TokenBuffer(name, TokenType.Use, pos.toPosision());
            }else if(name == "foreach")
            {
                return new TokenBuffer(name, TokenType.Foreach, pos.toPosision());
            }else if(name == "as")
            {
                return new TokenBuffer(name, TokenType.As, pos.toPosision());
            }else if(name == "new")
            {
                return new TokenBuffer(name, TokenType.New, pos.toPosision());
            }else if(name == "this")
            {
                return new TokenBuffer(name, TokenType.This, pos.toPosision());
            }else if(name == "self")
            {
                return new TokenBuffer(name, TokenType.Self, pos.toPosision());
            }else if(name == "while")
            {
                return new TokenBuffer(name, TokenType.While, pos.toPosision());
            }else if(name == "repeat")
            {
                return new TokenBuffer(name, TokenType.Repeat, pos.toPosision());
            }else if(name == "for")
            {
                return new TokenBuffer(name, TokenType.For, pos.toPosision());
            }

            return new TokenBuffer(name, TokenType.Variabel, this.pos.toPosision());
        }

        private int pop()
        {
            return pop(true);
        }

        private int pop(bool control)
        {
            int i = reader.Read();

            pos.nextRow();
            bp = pos.toPosision();

            if(i == '\n')
            {
                pos.nextLine();
                if(control)
                    return pop();
            }

            if (control && (i == '\r' || i == '\t' || i == ' ')) return pop();

            return i;
        }

        private int peek()
        {
            return reader.Peek();
        }

        private string toInt(int start)
        {
            StringBuilder context = new StringBuilder();
            context.Append((char)start);
            getInts(context);//wee get next int char :) 2 2 2 is 222 and so on :)

            if(peek() == '.')
            {
                //it is a comma :)
                context.Append((char)pop());
                getInts(context);//wee got number after , 
            }
            return context.ToString();
        }

        private void getInts(StringBuilder builder)
        {
            while (isInt(peek()))
                builder.Append((char)pop());
        }

        private TokenBuffer toString(int start)
        {
            int buffer;
            StringBuilder builder = new StringBuilder();

            while((buffer = peek()) != start && buffer != -1)
            {
                if (start == '"')
                {
                    if (buffer == '\\')
                    {
                        pop(false);
                        switch (peek())
                        {
                            case 'n':
                                builder.Append("\n");
                                break;
                            case 'r':
                                builder.Append("\r");
                                break;
                            case 't':
                                builder.Append("\t");
                                break;
                            case '\\':
                                builder.Append("\\");
                                break;
                            case '"':
                                builder.Append("\"");
                                break;
                            default:
                                return error("Unknown char after \\ (" + (char)peek() + ")", pos.toPosision());
                        }

                        pop(false);
                        continue;

                    }
                }
                else if (start == '\'' && buffer == '\\')
                {
                    int d = pop(false);
                    if(peek() == '\'')
                    {
                        builder.Append("'");
                        pop(false);
                    }
                    else
                    {
                        builder.Append((char)d);
                        builder.Append((char)pop(false));
                    }
                    continue;
                }

                    builder.Append((char)buffer);
                    pop(false);
                
            }

            if(buffer == -1)
            {
                return error("Missing (" + (char)start + ") got end of file", pos.toPosision());
            }
            pop();
            return new TokenBuffer(builder.ToString(), TokenType.String, pos.toPosision());
        }

        protected TokenBuffer error(string msg, Posision posision)
        {
            data.setError(new ScriptError(msg, posision), database);
            return new TokenBuffer("End of file", TokenType.EOF, posision);
        }
    }
}
