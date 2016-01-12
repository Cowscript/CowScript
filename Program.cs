using System;

namespace script
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write you code");
            Energy e = new Energy();
            try {
                e.parse(Console.ReadLine());
            }catch(ScriptError se)
            {
                Console.WriteLine("#Error");
                Console.WriteLine(se.Message);
                Console.WriteLine("On line [" + se.Posision.Line + "] Row [" + se.Posision.Row + "]");
            }

            Console.ReadLine();
        }
    }
}
