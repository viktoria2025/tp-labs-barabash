using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lab5
{
    class Program
    {
        // количество итераций, которые будем генерировать
        private const int TotalPoints = 100_000_000;

        static void Main(string[] args)
        {
            Console.WriteLine($"Доступно ядер: {Environment.ProcessorCount}");
            Console.WriteLine($"Всего точек: {TotalPoints:N0}");

            // 1. последовательная версия
            var sw = Stopwatch.StartNew();
            double seqPi = CalculatePiSequential(TotalPoints);
            sw.Stop();
            long seqTime = sw.ElapsedMilliseconds;
            Console.WriteLine($"\nПоследовательно: pi = {seqPi:F6}, время = {seqTime} мс");

            // 2. параллельная версия
            Console.Write("\nВведите количество потоков (например, 4): ");
            if (!int.TryParse(Console.ReadLine(), out int threadCount) || threadCount <= 0)
            {
                Console.WriteLine("Ошибка: нужно целое положительное число.");
                return;
            }

            // бонус: отмена по нажатию клавиши
            using var cts = new CancellationTokenSource();
            Console.WriteLine("\nНажмите 'C' для отмены вычислений...");

            // запускаем фоновую задачу, которая слушает нажатие клавиши
            _ = Task.Run(() =>
            {
                while (Console.ReadKey(true).Key != ConsoleKey.C) { }
                cts.Cancel();
            });

            sw.Restart();
            double parPi;
            try
            {
                parPi = CalculatePiParallel(TotalPoints, threadCount, cts.Token);
                sw.Stop();
                long parTime = sw.ElapsedMilliseconds;

                Console.WriteLine($"\nПараллельно ({threadCount} потоков): pi = {parPi:F6}, время = {parTime} мс");

                // расчет ускорения
                double speedup = (double)seqTime / parTime;
                Console.WriteLine($"Ускорение: {speedup:F2}x");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nВычисления отменены пользователем!");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // последовательный расчет числа пи
        private static double CalculatePiSequential(int points)
        {
            var random = new Random();
            int pointsInCircle = 0;

            for (int i = 0; i < points; i++)
            {
                double x = random.NextDouble();
                double y = random.NextDouble();

                // проверяем, попала ли точка в четверть круга
                if (x * x + y * y <= 1.0)
                {
                    pointsInCircle++;
                }
            }


            return 4.0 * pointsInCircle / points;
        }

        // параллельный расчет числа пи через Task
        private static double CalculatePiParallel(int points, int threadCount, CancellationToken token)
        {
            // разбиваем общее количество точек на равные части для каждого потока
            int pointsPerThread = points / threadCount;
            var tasks = new Task<int>[threadCount];

            for (int i = 0; i < threadCount; i++)
            {               
                // используем i + 1 как seed, чтобы числа не повторялись
                int threadId = i;

                tasks[i] = Task.Run(() =>
                {
                    var localRandom = new Random(threadId + 1);
                    int localCount = 0;

                    for (int j = 0; j < pointsPerThread; j++)
                    {
                        // бонус: проверяем, не отменили ли задачу
                        token.ThrowIfCancellationRequested();

                        double x = localRandom.NextDouble();
                        double y = localRandom.NextDouble();

                        if (x * x + y * y <= 1.0)
                        {
                            localCount++;
                        }
                    }
                    return localCount;
                }, token);
            }

            // ждем завершения всех задач и суммируем результаты
            Task.WaitAll(tasks, token);

            long totalInCircle = 0;
            foreach (var task in tasks)
            {
                totalInCircle += task.Result;
            }

            // если мы разбили нацело не делилось, нужно учесть остаток 
            return 4.0 * totalInCircle / points;
        }
    }
}