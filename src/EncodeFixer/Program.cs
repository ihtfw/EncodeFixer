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
        static void Main(string[] args)
        {
            string dir = null;

            if (args.Any())
            {
                foreach (var arg in args)
                {
                    if (!string.IsNullOrWhiteSpace(arg))
                    {
                        if (Directory.Exists(arg))
                        {
                            dir = arg;
                        }
                    }
                }
            }
            if (dir == null)
            {
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
                Encoding encoding;

                using (var fs = File.OpenRead(f))
                {
                    Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                    cdet.Feed(fs);
                    cdet.DataEnd();
                    if (cdet.Charset == null)
                    {
//                        Console.WriteLine($"{f} - Detection failed.");
                        continue;
                    }

                    switch (cdet.Charset)
                    {
                        case "ASCII":
                        case "UTF-8":
                            continue;
                        case "x-mac-cyrillic":
                        case "windows-1251":
                            encoding = Encoding.GetEncoding(1251);
                            break;
                        default:
                            Console.Out.WriteLine($"{cdet.Charset} - {f} - Skipped");
                            continue;
                    }
                }

//                Console.Out.WriteLine(f);
                var text = File.ReadAllText(f, encoding);
                File.WriteAllText(f, text, Encoding.UTF8);
            }
        }
    }
}
