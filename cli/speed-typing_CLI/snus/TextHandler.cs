using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// данный файл содержит класс Texthandler
namespace TextHandle
{
    // класс для считывания текста
    public static class TextHandler
    { 
        public static void ReadWords(string? filename, ValidWordConfig vwc, ref WordDifficultyMap wdm)
        {
            string? text;
            try
            {
                if (filename == null) return;

                text = File.ReadAllText(filename);

                if (text.Length == 0) throw new Exception("no data in file");

                string[] splitted = text.Split(" ");

                for (int i = 0; i < splitted.Length; i++)
                {
                    splitted[i] = splitted[i].Trim(vwc.CharsToRemove);
                    if (vwc.IsWordValid(splitted[i]))
                    {
                        wdm.Add(splitted[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error when opening a file: ");
                Console.WriteLine(ex);
                return;
            }
        } 
    }
}
