using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;


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
                Log.Information("Выбор вариантов ответа и ввод данных");
                Console.WriteLine("Выберите вариант команды:");
                Console.WriteLine("1. Добавить задачу. \n2. Удалить задачу. \n3. Все задачи. \n4. Выход из программы.");
                string выбор = Console.ReadLine();
                

                switch (выбор)
                {
                    case "1":
                        Log.Information("Производится добавление задачи");
                        Console.WriteLine("Введите название задачи:");
                        string название_задачи = Console.ReadLine();
                        AddTask(название_задачи);
                        break;

                    case "2":
                        Log.Information("Производится удаление задачи");
                        Console.WriteLine("Введите название задачи для удаления:");
                        string removeTitle = Console.ReadLine();
                        RemoveTask(removeTitle);
                        break;

                    case "3":
                        Log.Information("Производится вывод списка задач");
                        ListTasks();
                        break;
                    
                    case "4":
                        Log.Information("------Завершение работы программы------");
                        Environment.Exit(0);
                        break;

                    default:
                        Log.Information("Правильного ввода команды не произошло");
                        Console.WriteLine("Неверная команда. Пробуй еще раз.");
                        break;
                }
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