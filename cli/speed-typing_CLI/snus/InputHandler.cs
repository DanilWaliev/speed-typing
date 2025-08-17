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
        Watch,
        Tables,
        Help,
        Delete,
        Drop,
        Scan
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
                    case "scan":
                        if (commandArgs.Length > 1) return Command.Scan;
                        break;
                    case "watch":
                        if (commandArgs.Length > 0) return Command.Watch;
                        break;
                    case "delete":
                        if (commandArgs.Length > 1) return Command.Delete;
                        break;
                    case "drop":
                        if (commandArgs.Length > 0) return Command.Drop;
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
                watch <table name> - to watch words of specific table
                scan <file path> <table name> - to enter data from text file to specific table (will be created if not exists)
                drop <table name> - to delete specific table
                delete <table name> <word1 word2 word3 ...> - to delete words in specific table
                """;
            Console.WriteLine(helpText);
        }
    }
}
