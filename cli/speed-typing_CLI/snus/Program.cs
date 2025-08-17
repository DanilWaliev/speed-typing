using DB;
using TextHandle;

// TODO убрать вывод длины мапы если таблица не существует

namespace App
{
    class Program
    {
        public static void Main()
        {
            UI.InputHandler.Greet();

            // переменные для обработки ввода пользователя
            UI.Command? command;
            string?[] commandArgs;

            // основные перменные
            ValidWordConfig vwc = new ValidWordConfig();
            WordStorage ws = new WordStorage();
            WordDifficultyMap wdm = new WordDifficultyMap();


            while (true)
            {
                command = UI.InputHandler.GetCommand(out commandArgs);

                switch (command)
                {
                    case UI.Command.Exit:
                        return;
                    case UI.Command.Scan:
                        TextHandler.ReadWords(commandArgs[0], vwc, ref wdm);
                        ws.InsertWords(wdm, commandArgs[1]);
                        break;
                    case UI.Command.Watch:
                        wdm = ws.GetWords(commandArgs[0]);
                        wdm.Print();
                        break;
                    case UI.Command.Tables:
                        UI.InputHandler.ShowTables(ws.GetTableNames());
                        break;
                    case UI.Command.Help:
                        UI.InputHandler.ShowHelpInfo();
                        break;
                    case UI.Command.Delete:
                        string[] words = new string[commandArgs.Length - 1];
                        Array.Copy(commandArgs, 1, words, 0, commandArgs.Length - 1);
                        ws.DeleteWords(commandArgs[0], words);
                        break;
                    case UI.Command.Drop:
                        ws.DropTable(commandArgs[0]);
                        break;
                    default:
                        Console.WriteLine("unknown command");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}
