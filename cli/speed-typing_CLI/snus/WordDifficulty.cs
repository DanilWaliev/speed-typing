using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextHandle
{
    public struct WordDifficulty
    {
        public string Word { get; }
        public double Difficulty { get; }

        // инициализация модели клавиатуры
        static char[,] keyboardModel = new char[,]
        {
                {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-'},
                {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '['},
                {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\''},
                {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', '\\'}
        };

        // словарь для отображения координат каждого символа на модели клавиатуры
        static Dictionary<char, (int, int)> keyPosPairs = new Dictionary<char, (int, int)> { };

        static WordDifficulty()
        {
            // заполнение словаря
            for (int i = 0; i < keyboardModel.GetLength(0); i++)
            {
                for (int j = 0; j < keyboardModel.GetLength(1); j++)
                {
                    keyPosPairs.Add(keyboardModel[i, j], (i, j));
                }
            }
        }

        public WordDifficulty(string word)
        {
            Word = word;
            Difficulty = GetDifficulty(word);
        }

        public WordDifficulty(string word, double difficulty)
        {
            Word = word;
            Difficulty = difficulty;
        }

        private double GetDifficulty(string word)
        {
            double sum = 0;
            int centeredRowIndex = 1;
            int centeredColumnIndex = 5;
            int scoreForUpperChar = 5;

            for (int i = 0; i < word.Length; i++)
            {
                // добавление сложности за заглавную букву и создание копии с только строчными символами для работы с ней
                foreach (char c in word)
                {
                    if (Char.IsUpper(c))
                    {
                        sum += scoreForUpperChar;
                    }
                } 
                word = word.ToLower();
                

                (int row, int col) = keyPosPairs[word[i]];

                // добавление сложности за расстояние от центра клавиатуры
                sum += Math.Sqrt(Math.Pow(Math.Abs(col - centeredColumnIndex), 2) + Math.Pow(Math.Abs(row - centeredRowIndex), 2));

                // добавление сложности за расстояние между этим и предыдущим символом
                if (i != 0)
                {
                    (int prevRow, int prevCol) = keyPosPairs[word[i - 1]];
                    sum += Math.Sqrt(Math.Pow(Math.Abs(col - prevCol), 2) + Math.Pow(Math.Abs(row - prevRow), 2));
                }
            }

            return Math.Round((sum / word.Length), 3);
        }
    }

    public class WordDifficultyMap
    {
        // map всегда упорядоченна по алфавиту
        private List<WordDifficulty> map;

        private double maxDifficulty;
        private double minDifficulty;
        private double avgDifficulty;

        public WordDifficultyMap()
        {
            map = new List<WordDifficulty>();
        }

        public List<WordDifficulty> GetMap()
        {
            return map;
        }

        public WordDifficultyMap(List<WordDifficulty> map)
        {
            this.map = new List<WordDifficulty>(map);
        }
        public void Add(string word)
        {
            WordDifficulty wordDifficulty = new  WordDifficulty(word);
            int foundIndex = map.BinarySearch(wordDifficulty, new WordDifficultyComparer());

            if (foundIndex < 0) map.Insert(~foundIndex, wordDifficulty);      
        }

        public void Add(WordDifficulty wordDifficulty)
        {
            int foundIndex = map.BinarySearch(wordDifficulty, new WordDifficultyComparer());

            if (foundIndex < 0) map.Insert(~foundIndex, wordDifficulty);
        }

        public void Print(int numberOfChunk)
        { 
            int count = map.Count;
            int wordsPerChunk = 10;
            if (count == 0)
            {
                Console.WriteLine("no words");
            }

            int start = (numberOfChunk - 1) * wordsPerChunk;
            int end = ((numberOfChunk - 1) * wordsPerChunk) + wordsPerChunk;

            for (int i = start; i < end; i++)
            {
                Console.WriteLine(map[i].Word + " - " + map[i].Difficulty);
            }
            Console.WriteLine("[" + start +  "-" + end + "]");
        }

        public void PrintStats()
        {
            double max = 0;
            double min = 100;
            double sum = 0;
            double avg = 0;
            double step = 0.2;

            for (int i = 0; i < map.Count(); i++)
            {
                sum += map[i].Difficulty;
                if (max < map[i].Difficulty) max = map[i].Difficulty;
                if (min > map[i].Difficulty) min = map[i].Difficulty;
            }

            avg = sum / map.Count;

            min = Math.Round(min, 3);
            max = Math.Round(max, 3);
            avg = Math.Round(avg, 3);

            Console.WriteLine("count: " + map.Count());
            Console.WriteLine("min difficulty: " + min);
            Console.WriteLine("max difficulty: " + max);
            Console.WriteLine("avg difficulty: " + avg);

            int countOfIntervals = (int)((max - min) / step) + 1;
            for (int i = 0; i < countOfIntervals; i++)
            {
                int count = 0;
                double startOfInterval = Math.Round(min + (i * step), 3);
                double endOfInterval = Math.Round(min + ((i + 1) * step), 3);
                for (int j = 0; j < map.Count; j++)
                {
                    if (map[j].Difficulty >= startOfInterval && map[j].Difficulty <= endOfInterval) count++;
                }

                Console.WriteLine(startOfInterval + "-" + endOfInterval + ": " + count);
            }

        }
    }

    class WordDifficultyComparer : IComparer<WordDifficulty>
    {
        public int Compare(WordDifficulty w1, WordDifficulty w2)
        {
            return string.Compare(w1.Word, w2.Word);
        }
    }
}
