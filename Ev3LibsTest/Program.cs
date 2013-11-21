using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ev3Libs;

namespace Ev3LibsTest
{
    class Program
    {
        static Ev3Brick Brick;
        static string context;

        private delegate void Command(List<string> p);

        static Dictionary<string, Command> commandList = new Dictionary<string,Command>();
        static Dictionary<string, string> commandHelp = new Dictionary<string, string>();

        static void AddCommand(string cmd, string help, Command cmdDel)
        {
            commandList.Add(cmd, cmdDel);
            commandHelp.Add(cmd, help);
        }

        static void Main(string[] args)
        {
            bool Stop = false;

            AddCommand("port", "port set | open | close | check | read [port]", CommandPort);
            AddCommand("stop", "stop", delegate(List<string> p) { Stop = true;});
            AddCommand("list", "list", CommandList);
            
                        
            Brick = new Ev3Brick();
            
            context = "";

            do
            {
                List<string> command;

                Console.Write("\n" + context + ">");
                command = new List<string>(Console.ReadLine().Split(' '));

                if (!(command.Count == 0))
                {
                    if (commandList.ContainsKey(command[0]))
                    {
                        try
                        {
                            string c = command[0];
                            command.RemoveAt(0);
                            commandList[c].Invoke(command);
                        }
                        catch
                        {
                            Console.WriteLine("internal error - error in function definition");
                        }
                    }
                    else
                    {
                        Console.WriteLine("unknown command - use \"list\" to list known commands");
                    }
                }
            } while (!Stop);

        }

        static void CommandPort(List<string> p)
        {
            if (p.Count == 0)
            {
                Console.WriteLine("\"port\" requires parameters");
                Console.WriteLine("syntax : port set | open | close | check | read [port]");
                return;
            }

            if (p[0] == "set")
            {
                if (p.Count == 1)
                {
                    Console.WriteLine("\"port set\" requires a parameter");
                    Console.WriteLine("syntax : port set [port]");
                    return;
                }
                try
                {
                    Brick.SetPort(p[1]);
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }

            if (p[0] == "open")
            {
                try
                {
                    Brick.Connect();
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }

            if (p[0] == "close")
            {
                try
                {
                    Brick.Close();
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return; 
            }

            if (p[0] == "check")
            {
                try
                {
                    Console.WriteLine("Brick does" + (Brick.CheckHasData() ? "" : " not") + " have data available");
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }

            if (p[0] == "read")
            {
                try
                {
                    Console.WriteLine("Brick sent data : [" + Brick.GetStringData() + "]");
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }

            if (p[0] == "send")
            {
                if (p.Count == 1)
                {
                    Console.WriteLine("\"port send\" requires a parameter");
                    Console.WriteLine("syntax : port send [data]");
                    return;
                }
                try
                {
                    Brick.SendStringData(p[1]);
                }
                catch (Ev3Libs.Ev3Exceptions.ConnectionError e)
                {
                    Console.WriteLine(e.Message);
                }
                return;
            }

            Console.WriteLine("incorrect parameters");
            Console.WriteLine("syntax : port set | open | close | check | read [port]");
        }

        static void CommandList(List<string> p)
        {
            Console.WriteLine("known commands : ");
            foreach (string h in commandHelp.Values)
                Console.WriteLine(" " + h);
        }

        static void CommandWith(List<string> p)
        {
            if (p.Count == 0)
                p.Add("");

            context = p[0];
        }
    }
}
