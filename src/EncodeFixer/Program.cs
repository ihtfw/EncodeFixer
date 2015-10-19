using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodeFixer
{
    class Program
    {
        static Utf8Checker utf8Checker = new Utf8Checker();

        static void Main(string[] args)
        {
            string dir;

            while (true)
            {
                Console.WriteLine("Enter root dir for fix or 'abort' to exit:");
                dir = Console.ReadLine();
                if (dir == "abort")
                {
                    return;
                }

                if (string.IsNullOrEmpty(dir))
                {
                    Console.WriteLine("Dir cannot be empty");
                    continue;
                }

                if (!Directory.Exists(dir))
                {
                    Console.WriteLine("Dir not exist");
                    continue;
                }

                break;
            }

            DirSearch(dir);

            Console.WriteLine("---------------------END------------------------------");
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }

        static void DirSearch(string sDir)
        {
            try
            {
                ProcessFilesInDir(sDir);
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    ProcessFilesInDir(d);
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void ProcessFilesInDir(string d)
        {
            foreach (string f in Directory.GetFiles(d).Where(f => Path.GetExtension(f) == ".cs" || Path.GetExtension(f) == ".xaml"))
            {
                if (!utf8Checker.Check(f))
                {
                    Console.Out.WriteLine(f);
                    var text = File.ReadAllText(f, Encoding.GetEncoding(1251));
                    File.WriteAllText(f, text, Encoding.UTF8);
                }
            }
        }
    }
}
