namespace script.plugin.File
{
    class FileVariabelDatabase : VariabelDatabase
    {
        public VariabelDatabase VariabelDatabase { private set; get; }

        public FileVariabelDatabase(VariabelDatabase db)
        {
            VariabelDatabase = db;
        }
    }
}
