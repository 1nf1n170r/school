﻿using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lib
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(in int x, in int y)
        {
            X = x;
            Y = y;
        }
        public Point((int, int) tuple)
        {
            X = tuple.Item1;
            Y = tuple.Item2;
        }
    }
    public class Parser
    {
        public static T? Parse<T>(string val)
        {
            return (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(val);
        }
    }
    public class ConsoleTable
    {
        public class ConsoleTableBorders
        {
            public string TopLeftCorner { get; set; } = "┌";
            public string BottomLeftCorner { get; set; } = "└";
            public string TopRightCorner { get; set; } = "┐";
            public string BottomRightCorner { get; set; } = "┘";
            public string Cross { get; set; } = "┼";
            public string LeftTCross { get; set; } = "├";
            public string RightTCross { get; set; } = "┤";
            public string TopTCross { get; set; } = "┬";
            public string BottomTCross { get; set; } = "┴";
            public string Vertical { get; set; } = "│";
            public string Horizontal { get; set; } = "─";
        }
    }
    public static class Input
    {
        /// <summary>
        /// Ask for a string, depending on question
        /// </summary>
        /// <param name="qestion"></param>
        /// <param name="newline"></param>
        /// <param name="qu_color"></param>
        /// <param name="ans_color"></param>
        /// <returns></returns>
        public static T? Ask<T>(string qestion, bool newline, ConsoleColor quColor = ConsoleColor.DarkBlue, ConsoleColor ansColor = ConsoleColor.Green)
        {
            var std_color = Console.ForegroundColor;
            Console.ForegroundColor = quColor;
            if (newline) Console.WriteLine(">> " + qestion);
            else Console.Write(">> " + qestion);
            Console.ForegroundColor = ansColor;
            var ans = Console.ReadLine();
            Console.ForegroundColor = std_color;
            return Parser.Parse<T>(ans ?? "");
        }

        public static T AskCond<T>(string question, Func<string, bool> func, bool newline, Func<Exception, Exception?> exceptionHandler, ConsoleColor quColor = ConsoleColor.DarkBlue, ConsoleColor ansColor = ConsoleColor.Green, ConsoleColor errorColor = ConsoleColor.Red)
        {
            var originalColor = Console.ForegroundColor;

            while (true)
            {
                // Set the console color for the question
                Console.ForegroundColor = quColor;

                // Display the question
                if (newline)
                    Console.WriteLine(">> " + question);
                else
                    Console.Write(">> " + question);

                // Initialize variables for input handling
                StringBuilder inputBuilder = new();

                // Get the cursor position where the user starts typing
                int inputStartLeft = Console.CursorLeft;
                int inputStartTop = Console.CursorTop;

                // Set the console color for the answer (initially valid)
                Console.ForegroundColor = ansColor;

                while (true)
                {
                    // Read a key without displaying it automatically
                    var keyInfo = Console.ReadKey(intercept: true);
                    char keyChar = keyInfo.KeyChar;

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        var input = inputBuilder.ToString();
                        if (func(input))
                        {
                            T? parsedValue;
                            if ((parsedValue = Parser.Parse<T>(input)) != null)
                            {
                                Console.WriteLine();
                                Console.ForegroundColor = originalColor;
                                return parsedValue;
                            }
                        }
                        Console.WriteLine();
                        Exception? e;
                        if ((e = exceptionHandler(new ArgumentException("Invalid input"))) != null)
                            throw e;
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace && inputBuilder.Length > 0)
                    {
                        inputBuilder.Length--;
                        if (Console.CursorLeft == 0 && Console.CursorTop > 0)
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        else
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                    else if (!char.IsControl(keyChar))
                    {
                        inputBuilder.Append(keyChar);
                        Console.Write(keyChar);
                    }
                    string currentInput = inputBuilder.ToString();
                    bool isInputValid = func(currentInput);

                    if (isInputValid || currentInput.Length == 0)
                        Console.ForegroundColor = ansColor;
                    else
                        Console.ForegroundColor = errorColor;
                    // Move cursor back to the start of the input
                    Console.SetCursorPosition(inputStartLeft, inputStartTop);
                    Console.ForegroundColor = isInputValid ? ansColor : errorColor;
                    Console.Write(currentInput + " ");
                    Console.SetCursorPosition(inputStartLeft + currentInput.Length, inputStartTop);
                }

                // Restore the original console color before re-prompting
                Console.ForegroundColor = originalColor;
            }
        }
        /// <summary>
        /// Ask for key depending on question
        /// </summary>
        /// <param name="qestion"></param>
        /// <param name="newline"></param>
        /// <param name="print_val"></param>
        /// <param name="qu_color"></param>
        /// <param name="ans_color"></param>
        /// <returns></returns>
        public static ConsoleKey AskKey(string qestion, bool newline, bool printVal, ConsoleColor quColor = ConsoleColor.DarkBlue, ConsoleColor ansColor = ConsoleColor.Green)
        {
            var std_color = Console.ForegroundColor;
            Console.ForegroundColor = quColor;
            if (newline) Console.WriteLine(">> " + qestion);
            else Console.Write(">> " + qestion);
            Console.ForegroundColor = ansColor;
            var ans = Console.ReadKey(!printVal).Key;
            Console.WriteLine();
            Console.ForegroundColor = std_color;
            return ans;
        }

        public static Point DynMove(Point min, Point max)
        {
            var (x, y) = Console.GetCursorPosition();
            ConsoleKey key;
            while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (y - 1 >= min.Y) y--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (y + 1 <= max.Y) y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (x - 1 >= min.X) x--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (x + 1 <= max.X) x++;
                        break;
                }
                Console.SetCursorPosition(x, y);
            }
            return new Point(x, y);
        }

        public static MethodInfo GenerateMenu(Type type, Func<MethodInfo, bool> expr, Func<string, string> name_gen)
        {
            var origin = new Point(Console.GetCursorPosition());
            var flags = BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var items = type.GetMethods(flags).Where(expr).ToArray();
            foreach (var item in items)
                Console.WriteLine(name_gen(item.Name));
            Console.SetCursorPosition(origin.X, origin.Y);
            var move = DynMove(new Point(origin.X, origin.Y), new Point(origin.X, origin.Y + items.Length - 1));
            var delta_index = Math.Abs(origin.Y - move.Y);
            return items[delta_index];
        }
    }
}
/*
public class Program
{
    static void main()
    {
        throw new DatabaseException("This is happening on the database", new JsonParseException("Json didnt parse correctly"));
    }
}
public class DatabaseException : Exception
{
    public DatabaseException(string msg, Exception inner) : base(msg, inner)
    {

    }
    public DatabaseException(string msg) : base(msg)
    {

    }
}
public class JsonParseException : Exception
{
    public JsonParseException(string msg, Exception inner) : base(msg, inner)
    {

    }
    public JsonParseException(string msg) : base(msg)
    {

    }
}
public class HttpRequestException : Exception
{
    public HttpRequestException(string msg, Exception inner) : base(msg, inner)
    {

    }
    public HttpRequestException(string msg) : base(msg)
    {

    }
}
*/