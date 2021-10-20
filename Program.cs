using System;
using System.Collections.Generic;
using System.IO;

namespace pstCopy
{
    class Program
    {
        static private List<string> files = new List<string>();
        static string path;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Listendatei angeben:");
                try
                {
                    foreach (string line in File.ReadAllText(Console.ReadLine()).Split('\n'))
                        files.Add(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler:\nDie Datei konnte nicht gefunden oder geöffnet werden!\n\n" + ex.ToString());
                    return;
                }

                Console.WriteLine("Wo sollen die Dateien hin gespeichert werden:");
                path = Console.ReadLine();
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Fehler:\nEs müssen 2 Argumente angegeben werden; pstCopy.exe [txt-Datei mit Liste der pst-Dateien] [Ziel-Pfad]");
                    return;
                }
                else
                {
                    try
                    {
                        foreach (string line in File.ReadAllText(args[0]).Split('\n'))
                            files.Add(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Fehler:\nDie Datei konnte nicht gefunden oder geöffnet werden!\n\n" + ex.ToString());
                        return;
                    }
                    path = args[1];
                    if(!Directory.Exists(path))
                    {
                        Console.WriteLine("Fehler:\nDer angegebene Zielpfad existiert nicht!");
                        return;
                    }
                }
            }

            foreach (string file in files)
            {
                if (file.Length < 3)
                    continue;

                string fileName = "", outFile = "";
                if (file.StartsWith(@"\\nt-nas01\pst"))
                {
                    fileName = file.Substring(15).Split('\\')[0];
                    for(int i = 1; ;i++)
                        if(!File.Exists(path + "\\" + fileName + "\\" + fileName + "_" + i.ToString("00") + ".pst"))
                        {
                            fileName += "_" + i.ToString("00") + ".pst";
                            break;
                        }
                    outFile = file;
                }
                else if(file.StartsWith(@"E:\home"))
                {
                    fileName = file.Substring(8).Split('\\')[0];
                    for (int i = 1; ; i++)
                        if (!File.Exists(path + "\\" + fileName + "\\" + fileName + "_" + i.ToString("00") + ".pst"))
                        {
                            fileName += "_" + i.ToString("00") + ".pst";
                            break;
                        }
                    outFile = @"\\nt-file\home$\" + file.Substring(8);
                }
                else
                {
                    for (int i = 1; ; i++)
                        if (!File.Exists(path + "\\" + "Error" + i.ToString("00") + ".pst"))
                        {
                            fileName = "Error" + i.ToString("00") + ".pst";
                            break;
                        }
                    outFile = file;
                }

                if (!Directory.Exists(path + "\\" + fileName.Substring(0, fileName.Length - 7))) Directory.CreateDirectory(path + "\\" + fileName.Substring(0, fileName.Length - 7));
                if (!File.Exists(Environment.CurrentDirectory + "\\log.txt")) File.Create(Environment.CurrentDirectory + "\\log.txt");
                Console.WriteLine("\n" + DateTime.Now.ToString("dd.mm.yyy HH:MM:ss,ff") + "\n" + outFile + "\n->\n" + path + "\\" + fileName.Substring(0, fileName.Length-7) + "\\" + fileName + "\nCopying...\n");
                File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", File.ReadAllText(Environment.CurrentDirectory + "\\log.txt") + "\n" + DateTime.Now.ToString("dd.mm.yyy HH:MM:ss,ff") + "\n" + outFile + "\n->\n" + path + "\\" + fileName.Substring(0, fileName.Length - 7) + "\\" + fileName + "\nCopying...\n");
                try { File.Move(file, path + "\\" + fileName.Substring(0, fileName.Length - 7) + "\\" + fileName); }
                catch (IOException e) 
                { 
                    Console.WriteLine("\n\n!!!Fehler beim verschieben der Datei aufgetreten\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                    File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", File.ReadAllText(Environment.CurrentDirectory + "\\log.txt") + "\n" + DateTime.Now.ToString("dd.mm.yyy HH:MM:ss,ff") + "\n\n!!!Fehler beim verschieben der Datei aufgetreten\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                }
                catch (UnauthorizedAccessException e) 
                {
                    Console.WriteLine("\n\n!!!Auf die Datei konnte nicht zugegriffen werden\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                    File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", File.ReadAllText(Environment.CurrentDirectory + "\\log.txt") + "\n" + DateTime.Now.ToString("dd.mm.yyy HH:MM:ss,ff") + "\n\n!!!Auf die Datei konnte nicht zugegriffen werden\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                }
                catch(Exception e)
                {
                    Console.WriteLine("\n\n!!!Ein unbekannter Fehler ist aufgetreten\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                    File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", File.ReadAllText(Environment.CurrentDirectory + "\\log.txt") + "\n" + DateTime.Now.ToString("dd.mm.yyy HH:MM:ss,ff") + "\n\n!!!Ein unbekannter Fehler ist aufgetreten\n" + e.GetBaseException().ToString() + "\n\nDatei wird übersprungen!");
                }
            }

            Console.WriteLine("\nZum Beenden Taste drücken...");
            Console.ReadKey();
        }
    }
}
