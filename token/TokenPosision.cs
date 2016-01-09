namespace script.token
{
    public class TokenPosision
    {
        private int row, line;

        public TokenPosision()
        {
            row = 0;
            line = 1;
        }

        public void nextRow()
        {
            row++;
        }

        public void unsetRow()
        {
            row = 0;
        }

        public void nextLine()
        {
            line++;
            unsetRow();
        }

        public Posision toPosision()
        {
            return new Posision(this.line, this.row);
        }
    }
}
