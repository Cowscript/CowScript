using System;

namespace script
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write you code");
            Energy e = new Energy();
            e.parse(Console.ReadLine());
            if(e.getRunningStatus() == RunningState.Error){
                ScriptError se = e.getError();
                Console.WriteLine("#Error");
                Console.WriteLine(se.Message);
                Console.WriteLine("On line [" + se.Posision.Line + "] Row [" + se.Posision.Row + "]");
            }

            Console.ReadLine();
        }
    }
}
