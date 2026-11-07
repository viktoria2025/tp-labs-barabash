using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    // базовый абстрактный класс билета, реализует сравнение для сортировки
    public abstract class Ticket : IComparable<Ticket>
    {
        // приватные поля для инкапсуляции
        private readonly string _movieName;
        private readonly decimal _basePrice;
        private readonly int _row;
        private readonly int _seat;

        // свойства для безопасного доступа к полям
        public string MovieName => _movieName;
        public decimal BasePrice => _basePrice;
        public int Row => _row;
        public int Seat => _seat;

        // конструктор с проверкой корректности аргументов
        protected Ticket(string movieName, decimal basePrice, int row, int seat)
        {
            if (string.IsNullOrWhiteSpace(movieName))
                throw new ArgumentException("название фильма не может быть пустым.", nameof(movieName));
            if (basePrice < 0)
                throw new ArgumentOutOfRangeException(nameof(basePrice), "цена не может быть отрицательной.");
            if (row <= 0 || seat <= 0)
                throw new ArgumentException("ряд и место должны быть положительными числами.");

            _movieName = movieName;
            _basePrice = basePrice;
            _row = row;
            _seat = seat;
        }

        // абстрактный метод, который обязаны переопределить наследники
        public abstract decimal CalculatePrice();

        // реализация интерфейса для сортировки билетов по цене
        public int CompareTo(Ticket? other)
        {
            if (other == null) return 1;
            return CalculatePrice().CompareTo(other.CalculatePrice());
        }

        // переопределение метода для красивого вывода
        public override string ToString()
        {
            return $"{GetType().Name} | фильм: {MovieName} | место: {Row} ряд, {Seat} место | цена: {CalculatePrice()} руб.";
        }
    }

    // класс обычного билета
    public class StandardTicket : Ticket
    {
        public StandardTicket(string movieName, decimal basePrice, int row, int seat)
            : base(movieName, basePrice, row, seat) { }

        public override decimal CalculatePrice() => BasePrice;
    }

    // класс vip билета (цена умножается на 2)
    public class VipTicket : Ticket
    {
        public VipTicket(string movieName, decimal basePrice, int row, int seat)
            : base(movieName, basePrice, row, seat) { }

        public override decimal CalculatePrice() => BasePrice * 2.0m;

        // дополнительное переопределение для вывода пометки
        public override string ToString()
        {
            return base.ToString() + " (vip-зона)";
        }
    }

    // класс льготного билета (скидка 50 процентов)
    public class DiscountTicket : Ticket
    {
        public DiscountTicket(string movieName, decimal basePrice, int row, int seat)
            : base(movieName, basePrice, row, seat) { }

        public override decimal CalculatePrice() => BasePrice * 0.5m;
    }

    // класс-менеджер для управления продажами и сбора статистики
    public class Cinema
    {
        // коллекция для полиморфной обработки
        private readonly List<Ticket> _tickets = new List<Ticket>();

        public int HallCapacity { get; }

        public Cinema(int hallCapacity)
        {
            if (hallCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(hallCapacity));
            HallCapacity = hallCapacity;
        }

        // метод продажи билета с проверкой вместимости зала
        public void SellTicket(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (_tickets.Count >= HallCapacity)
                throw new InvalidOperationException("зал полностью заполнен!");
            _tickets.Add(ticket);
        }

        // подсчет общей выручки через linq
        public decimal GetTotalRevenue() => _tickets.Sum(t => t.CalculatePrice());

        // вычисление процента занятости зала
        public double GetOccupancyPercentage() => (double)_tickets.Count / HallCapacity * 100;

        // вывод всей статистики сеанса
        public void PrintSessionStatistics()
        {
            Console.WriteLine("\nСтатистика:");
            Console.WriteLine($"всего продано билетов: {_tickets.Count} из {HallCapacity} мест.");
            Console.WriteLine($"занятость зала: {GetOccupancyPercentage():F1}%");
            Console.WriteLine($"общая выручка: {GetTotalRevenue()} руб.");

            Console.WriteLine("\nсписок проданных билетов:");

            // сортировка списка благодаря IComparable
            _tickets.Sort();

            // полиморфный цикл без проверок типов
            foreach (var ticket in _tickets)
            {
                Console.WriteLine(ticket);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // создаем кинотеатр на 50 мест
            var cinema = new Cinema(50);

            try
            {
                // создаем 6 объектов разных типов
                cinema.SellTicket(new StandardTicket("дюна: часть 2", 400m, 5, 10));
                cinema.SellTicket(new StandardTicket("дюна: часть 2", 400m, 5, 11));
                cinema.SellTicket(new VipTicket("дюна: часть 2", 400m, 10, 1));
                cinema.SellTicket(new DiscountTicket("дюна: часть 2", 400m, 7, 15));
                cinema.SellTicket(new StandardTicket("дюна: часть 2", 400m, 8, 12));
                cinema.SellTicket(new VipTicket("дюна: часть 2", 400m, 10, 2));

                // выводим статистику
                cinema.PrintSessionStatistics();
            }
            catch (Exception ex)
            {
                // обработка возможных ошибок
                Console.WriteLine($"ошибка: {ex.Message}");
            }

            Console.WriteLine("\nнажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}