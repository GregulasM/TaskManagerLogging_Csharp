using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

// ------ Информация о программе и всяких модулях ------ // 

// Чем отличается от остальных?
// Тут имеется чуть более комплексная реализация Serilog в виде Log.Error, Log.Fatal, Log.Warning и т.п.
// Добавил stopwatch на разные функции (stopwatch_add_task, stopwatch_main и т.п.) для отслеживания времени выполнения. Время работы программы тоже отслеживается (￣ω￣)
// Имеется небольшой отлов ошибок, плюсом собственные классы для отлова (ಠ_ಠ). Дополнительно записывается StackTrace для ошибок.

// ------------------------------------------------ // 

namespace TaskManager
{
    // Модель задачи для списка. Не более.
    public class Task
    {
        public string Title { get; set; }

        public Task(string название_задачи)
        {
            Title = название_задачи;
        }
    }

    class Program
    {
        // Список задач
        private static List<Task> tasks = new List<Task>();

        static void Main(string[] args)
        {

            try
            {

                Stopwatch stopwatch_main = Stopwatch.StartNew();


                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                Log.Information("------Приложение успешно запущено (через appsettings.json)------");

                Log.Information("Запуск цикла программы");

                // Крутим приложение в цикле и даем на выбор несколько вариантов взаимодействия. При выходе завершаем работу программы. Вводим цифры, 1, 2, 3, 4 для работы.
                while (true)
                {
                    try
                    {
                        
                    

                    Log.Information("Выбор вариантов ответа и ввод данных");
                    Console.WriteLine("Выберите вариант команды:");
                    Console.WriteLine(
                        "1. Добавить задачу. \n2. Удалить задачу. \n3. Все задачи. \n4. Выход из программы.");
                    string выбор = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(выбор))
                    {
                        throw new NullOrWhiteSpace("Ввод при выборе задачи пуст.");
                    }


                    switch (выбор)
                    {
                        case "1":
                            Log.Information("Производится добавление задачи");
                            Console.WriteLine("Введите название задачи:");
                            string название_задачи = Console.ReadLine();
                            Stopwatch stopwatch_add_task = Stopwatch.StartNew();
                            AddTask(название_задачи);
                            stopwatch_add_task.Stop();
                            Log.Information($"Время работы AddTask(): {stopwatch_add_task.ElapsedMilliseconds} ms");
                            break;

                        case "2":
                            Log.Information("Производится удаление задачи");
                            Console.WriteLine("Введите название задачи для удаления:");
                            string removeTitle = Console.ReadLine();
                            Stopwatch stopwatch_remove_task = Stopwatch.StartNew();
                            RemoveTask(removeTitle);
                            stopwatch_remove_task.Stop();
                            Log.Information(
                                $"Время работы RemoveTask(): {stopwatch_remove_task.ElapsedMilliseconds} ms");
                            break;

                        case "3":
                            Log.Information("Производится вывод списка задач");
                            Stopwatch stopwatch_list_task = Stopwatch.StartNew();
                            ListTasks();
                            stopwatch_list_task.Stop();
                            Log.Information($"Время работы ListTasks(): {stopwatch_list_task.ElapsedMilliseconds} ms");
                            break;

                        case "4":
                            Log.Information("Производится выход из программы");
                            stopwatch_main.Stop();
                            Log.Information(
                                $"Общее время работы программы: {stopwatch_main.ElapsedMilliseconds} мс ({stopwatch_main.ElapsedMilliseconds / 1000} сек.)");
                            Log.Information("------Завершение работы программы------");
                            Environment.Exit(0);
                            break;

                        default:
                            Log.Warning("Правильного ввода команды не произошло");
                            Console.WriteLine("Неверная команда. Пробуй еще раз.");
                            break;
                    }
                    }
                    catch(NullOrWhiteSpace ex)
                    {
                        Log.Write(LogEventLevel.Error, ex.ToString());
                        // Log.Fatal(ex.StackTrace);
                        // Log.Fatal(ex.Message);
                    }
                    catch(TaskLenghtMinimal ex)
                    {
                        Log.Write(LogEventLevel.Error, ex.ToString());

                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Непредвиденная некритичная ошибка программы.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Фатальная ошибка программы");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Log.Write(LogEventLevel.Fatal, ex.ToString());
                Environment.Exit(1);
            }
            
        }

        // Добавление задачи в список. Проверка на дубликаты отсутствует. Имеется проверка IsNullOrWhiteSpace.
        private static void AddTask(string название_задачи)
        {
            Log.Information("Начало операции AddTask()");
            if (string.IsNullOrWhiteSpace(название_задачи))
            {
                Log.Warning("Попытка добавить задачу с пустым названием. Операция не выполнена");
                Log.Information("Конец добавления задачи AddTask()");
                return;
            }
            else if (название_задачи.Length < 2)
            {
                throw new TaskLenghtMinimal("Минимальная длина названия задачи: 2 символа.");
            }

            Task newTask = new Task(название_задачи);
            tasks.Add(newTask);
            Console.WriteLine($"Задача «{название_задачи}» добавлена.");
            
            Log.Information($"Задача «{название_задачи}» успешно добавлена");
            Log.Information("Конец операции AddTask()");
        }

        // Удаление задачи из списка благодаря LinQ- производим поиск, удаляем совпадения. Если не нашли, то держим в курсе, что такой задачи не нашли.
        private static void RemoveTask(string название_задачи)
        {
            Log.Information("Начало операции RemoveTask()");
            Task taskToRemove = tasks.Find(t => t.Title == название_задачи);
            if (taskToRemove == null)
            {
                Log.Error($"Задача «{название_задачи}» не найдена.");
                Log.Information("Конец операции RemoveTask()");
                return;
            }

            tasks.Remove(taskToRemove);
            Log.Information($"Задача «{название_задачи}» операции RemoveTask() успешно удалена.");
            Log.Information("Конец операции RemoveTask()");
        }

        // Проверка списка на наличие задач - если есть, то выводим их в цикле, если нет, то говорим, что пусто.
        private static void ListTasks()
        {
            Log.Information("Начало операции ListTasks()");
            if (tasks.Count == 0)
            {
                Log.Information("Выведенный список задач пуст");
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                Log.Information($"Всего выведено задач: {tasks.Count}");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i].Title}");
                }

                Log.Information("{@Tasks}", tasks);
            }
            Log.Information("Конец операции ListTasks()");
        }
    }
}