using script.help;
using script.token;
using System.IO;

namespace script.plugin.File
{
    class FileEnergy
    {
        public static void parse(EnegyData data, VariabelDatabase database, string file)
        {
            StartItems.CreateStartItems(data, database);
            using (FileStream reader = System.IO.File.OpenRead(file))
            {
                using(StreamReader r = new StreamReader(reader))
                {
                    Interprenter.parse(new Token(r, data, database), data, database, true);
                }
            }
        }
    }
}
