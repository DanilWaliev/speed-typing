using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public enum Command
    {
        Exit,
        Words,
        Tables,
        Help,
        Delete,
        Drop,
        Read,
        Stat
    }

    internal static class InputHandler
    {
        public static void Greet()
        {
            string version = "1.0 alpha";
            string greet = "welcome to snus (scan new unique strings)";

            Console.WriteLine(greet + " " + version);
            Console.WriteLine("type \"help\" for information about command");
        }

        public static void ShowTables(List<string> tables)
        {
            Console.WriteLine("tables:");
            foreach (string table in tables)
            {
                Console.WriteLine(table);
            }
        }

        public static Command GetCommand(out string[] commandArgs)
        {
            Console.Write("> ");

            while (true)
            {
                string? command = Console.ReadLine();
                if (command == null) continue;

                string[] splittedCommand = command.Split(" ");

                commandArgs = new string[splittedCommand.Length - 1];

                Array.Copy(splittedCommand, 1, commandArgs, 0, splittedCommand.Length - 1);

                switch (splittedCommand[0].ToLower())
                {
                    case "read":
                        if (commandArgs.Length > 1) return Command.Read;
                        break;
                    case "words":
                        if (commandArgs.Length > 1 && Int32.Parse(commandArgs[1]) > 0 && Int32.TryParse(commandArgs[1], out int _)) return Command.Words;
                        break;
                    case "delete":
                        if (commandArgs.Length > 1) return Command.Delete;
                        break;
                    case "drop":
                        if (commandArgs.Length > 0) return Command.Drop;
                        break;
                    case "stat":
                        if (commandArgs.Length > 0) return Command.Stat;
                        break;
                    case "tables":
                        return Command.Tables;
                    case "exit":
                        return Command.Exit;
                    case "help":
                        return Command.Help;
                    default:
                        continue;
                }
            }
        }

        public static void ShowHelpInfo()
        {
            string helpText = """
                tables - to watch list of tables
                words <table name> <number of chunk> - to watch words of specific table in specific chunk
                read <file path> <table name> - to enter data from text file to specific table (will be created if not exists)
                drop <table name> - to delete specific table
                delete <table name> <word1 word2 word3 ...> - to delete words in specific table
                stat <table name> - to watch statistic about specific table
                """;
            Console.WriteLine(helpText);
        }
    }
}
