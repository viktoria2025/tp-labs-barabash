using System;
using System.Globalization;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Задание 1");
                Console.WriteLine("Задание 2");
                Console.WriteLine("Задание 3");
                Console.WriteLine("Задание 4");
                Console.WriteLine("Выход");
                Console.Write("Выберите пункт меню");

                string? userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        ExecuteTaskOne();
                        break;
                    case "2":
                        ExecuteTaskTwo();
                        break;
                    case "3":
                        ExecuteTaskThree();
                        break;
                    case "4":
                        ExecuteTaskFour();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ошибка: выбран несуществующий пункт меню.");
                        break;
                }
            }
        }

        // задание 1
        private static void ExecuteTaskOne()
        {
            Console.Write("Введите целое число n (от 0 до 20): ");

            if (!int.TryParse(Console.ReadLine(), out int n) || n < 0 || n > 20)
            {
                Console.WriteLine("Ошибка: необходимо ввести целое число в диапазоне от 0 до 20.");
                return;
            }

            long factorial = CalculateFactorial(n);
            Console.WriteLine($"Факториал {n}! = {factorial}");
        }

        private static long CalculateFactorial(int n)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        // задание 2
        private static void ExecuteTaskTwo()
        {
            Console.Write("Введите значение a: ");

            // используем InvariantCulture, чтобы точка всегда работала как разделитель
            if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double a))
            {
                Console.WriteLine("Ошибка: введено некорректное числовое значение.");
                return;
            }

            double result = CalculateTaskTwoFormula(a);
            Console.WriteLine($"Результат: {result}");
        }

        private static double CalculateTaskTwoFormula(double a)
        {
            return 3 * a * a;
        }

        // задание 3
        private static void ExecuteTaskThree()
        {
            Console.Write("Введите значение x: ");

            if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
            {
                Console.WriteLine("Ошибка: введено некорректное числовое значение.");
                return;
            }

            double result = CalculateTaskThreeFormula(x);
            Console.WriteLine($"Результат: {result}");
        }

        private static double CalculateTaskThreeFormula(double x)
        {
            return Math.Pow(x, 3) - 2 * x;
        }

        // задание 4 (вариант 13)
        private static void ExecuteTaskFour()
        {
            Console.Write("Введите значение x: ");
            if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
            {
                Console.WriteLine("Ошибка: введено некорректное значение x.");
                return;
            }

            Console.Write("Введите точность epsilon (например, 0.0001): ");
            if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double epsilon) || epsilon <= 0)
            {
                Console.WriteLine("Ошибка: точность должна быть положительным числом.");
                return;
            }

            double result = CalculateExpTaylor(x, epsilon);
            Console.WriteLine($"e^{x} = {result}");
        }

        private static double CalculateExpTaylor(double x, double epsilon)
        {
            double sum = 1.0; // первый член ряда
            double currentTerm = 1.0; // текущий член ряда
            int n = 1;

            // рекуррентно вычисляем следующий член: a_n = a_{n-1} * x / n
            do
            {
                currentTerm *= x / n;
                sum += currentTerm;
                n++;
            }
            while (Math.Abs(currentTerm) > epsilon);

            return sum;
        }
    }
}