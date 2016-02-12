using script.token;
using script.variabel;
using System.Collections;

namespace script.parser
{
    class ForParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token, bool isFile)
        {
            if(token.next().type() != TokenType.LeftBue)
            {
                ed.setError(new ScriptError("Missing ( after 'for'", token.getCache().posision()), db);
                return null;
            }

            //push the tokens until ;
            ArrayList init = getNextBlock(ed, db, token, true);

            if (ed.State != RunningState.Normal)
            {
                return null;
            }

            ArrayList status = getNextBlock(ed, db, token, true);

            if (ed.State != RunningState.Normal)
            {
                return null;
            }

            ArrayList handler = getNextBlock(ed, db, token, false);

            if (ed.State != RunningState.Normal)
            {
                return null;
            }

            ArrayList body = BodyParser.parse(token, ed, db);

            if (ed.State != RunningState.Normal)
            {
                return null;
            }

            //init the data :)
            new VariabelParser().parseNoEnd(ed, db, new TokenCache(init, ed, db));

            while(ed.State == RunningState.Normal && new VariabelParser().parseNoEnd(ed, db, new TokenCache(status, ed, db)).toBoolean(token.getCache().posision(), ed, db))
            {
                Interprenter.parse(new TokenCache(body, ed, db), ed, db);
                if (ed.State == RunningState.Normal)
                    new VariabelParser().parseNoEnd(ed, db, new TokenCache(handler, ed, db));
            }
            token.next();
            return null;
        }

        private ArrayList getNextBlock(EnegyData data, VariabelDatabase db, Token token, bool isEnd)
        {
            TokenBuffer buffer;
            ArrayList b = new ArrayList();
            while((buffer = token.next()).type() != TokenType.EOF && buffer.type() != (isEnd ? TokenType.End : TokenType.RightBue))
            {
                b.Add(buffer);
            }

            if(token.getCache().type() != (isEnd ? TokenType.End : TokenType.RightBue))
            {
                data.setError(new ScriptError("Missing "+(isEnd ? ";" : ")")+" got: " + token.getCache().ToString(), token.getCache().posision()), db);
                return new ArrayList();
            }

            return b;
        }
    }
}
