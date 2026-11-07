using System;
using System.IO;
using System.Threading.Tasks;

namespace Lab3
{
    // класс для работы с журналом событий
    public class EventJournal
    {
        // путь к файлу журнала
        private readonly string _filePath;

        // конструктор проверяет и сохраняет путь
        public EventJournal(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("путь к файлу не может быть пустым.", nameof(filePath));
            _filePath = filePath;
        }

        // асинхронное добавление записи в файл
        public async Task AddEntryAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("текст записи не может быть пустым.", nameof(text));

            // формируем строку с меткой времени
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}{Environment.NewLine}";

            // дозапись в конец файла асинхронно
            await File.AppendAllTextAsync(_filePath, entry);
        }

        // асинхронное чтение записей за указанную дату (бонусное задание)
        public async Task<string> GetEntriesByDateAsync(string date)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("файл журнала не найден.");

            // читаем весь текст асинхронно
            string allText = await File.ReadAllTextAsync(_filePath);

            string[] lines = allText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";

            foreach (string line in lines)
            {
                // ищем строки, которые начинаются с нужной даты
                if (line.StartsWith($"[{date}"))
                {
                    result += line + Environment.NewLine;
                }
            }

            return result;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // файл журнала лежит рядом с программой
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "journal.txt");
            var journal = new EventJournal(filePath);

            while (true)
            {
                Console.WriteLine("\nЖурнал событий.");
                Console.WriteLine("Добавить запись");
                Console.WriteLine("Просмотреть записи за дату");
                Console.WriteLine("Выход");
                Console.Write("выберите действие: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("введите текст записи: ");
                            string text = Console.ReadLine();
                            await journal.AddEntryAsync(text);
                            Console.WriteLine("запись успешно добавлена.");
                            break;

                        case "2":
                            Console.Write("введите дату в формате ГГГГ-ММ-ДД: ");
                            string date = Console.ReadLine();
                            string entries = await journal.GetEntriesByDateAsync(date);

                            if (string.IsNullOrEmpty(entries))
                                Console.WriteLine("записей за эту дату не найдено.");
                            else
                                Console.WriteLine($"\nзаписи за {date}:\n{entries}");
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("неверный пункт меню.");
                            break;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"ошибка: {ex.Message}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("ошибка: нет прав на доступ к файлу.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"ошибка ввода-вывода: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"непредвиденная ошибка: {ex.Message}");
                }
            }
        }
    }
}