namespace script
{
    public class Posision
    {
        public int Line { private set; get; }
        public int Row { private set; get; }

        public Posision(int line, int row)
        {
            this.Line = line;
            this.Row = row;
        }
    }
}
