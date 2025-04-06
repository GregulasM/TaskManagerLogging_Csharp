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

            // Задаем вывод в консоль и в текстовый файл, делаем автозапись в файл.
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener("TaskManager.txt"));
            Trace.AutoFlush = true;
            Trace.TraceInformation("[INFO] Программа TaskManager запущена.");

            // Крутим приложение в цикле и даем на выбор несколько вариантов взаимодействия. При выходе завершаем работу программы. Вводим цифры, 1, 2, 3, 4 для работы.
            while (true)
            {
                Console.WriteLine("Выберите вариант команды:");
                Console.WriteLine("1. Добавить задачу. \n2. Удалить задачу. \n3. Все задачи. \n4. Выход из программы.");
                string выбор = Console.ReadLine();
                

                switch (выбор)
                {
                    case "1":
                        Console.WriteLine("Введите название задачи:");
                        string название_задачи = Console.ReadLine();
                        AddTask(название_задачи);
                        break;

                    case "2":
                        Console.WriteLine("Введите название задачи для удаления:");
                        string removeTitle = Console.ReadLine();
                        RemoveTask(removeTitle);
                        break;

                    case "3":
                        ListTasks();
                        break;
                    
                    case "4":
                        Trace.TraceInformation("[INFO] Завершение работы программы.");
                        Trace.WriteLine("---------------------------------------------");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Неверная команда. Пробуй еще раз.");
                        break;
                }
            }
        }

        // Добавление задачи в список. Проверка на дубликаты отсутствует. Имеется проверка IsNullOrWhiteSpace.
        private static void AddTask(string название_задачи)
        {
            Trace.WriteLine("[TRACE] Начало операции AddTask.");
            if (string.IsNullOrWhiteSpace(название_задачи))
            {
                Trace.TraceWarning("[WARN] Попытка добавить задачу с пустым названием. Операция не выполнена.");
                Trace.WriteLine("[TRACE] Конец операции AddTask.");
                return;
            }

            Task newTask = new Task(название_задачи);
            tasks.Add(newTask);
            Trace.TraceInformation($"[INFO] Задача «{название_задачи}» успешно добавлена.");
            Trace.WriteLine("[TRACE] Конец операции AddTask.");
        }

        // Удаление задачи из списка благодаря LinQ- производим поиск, удаляем совпадения. Если не нашли, то держим в курсе, что такой задачи не нашли.
        private static void RemoveTask(string название_задачи)
        {
            Trace.WriteLine("[TRACE] Начало операции RemoveTask.");
            Task taskToRemove = tasks.Find(t => t.Title == название_задачи);
            if (taskToRemove == null)
            {
                Trace.TraceError($"[ERROR] Задача «{название_задачи}» не найдена.");
                Trace.WriteLine("[TRACE] Конец операции RemoveTask.");
                return;
            }

            tasks.Remove(taskToRemove);
            Trace.TraceInformation($"[INFO] Задача «{название_задачи}» успешно удалена.");
            Trace.WriteLine("[TRACE] Конец операции RemoveTask.");
        }

        // Проверка списка на наличие задач - если есть, то выводим их в цикле, если нет, то говорим, что пусто.
        private static void ListTasks()
        {
            Trace.WriteLine("[TRACE] Начало операции ListTasks.");
            if (tasks.Count == 0)
            {
                Trace.TraceInformation("[INFO] Список задач пуст.");
                Console.WriteLine("Список задач пуст.");
            }
            else
            {
                Trace.TraceInformation($"[INFO] Всего задач: {tasks.Count}.");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tasks[i].Title}");
                }
            }
            Trace.WriteLine("[TRACE] Конец операции ListTasks.");
        }
    }
}