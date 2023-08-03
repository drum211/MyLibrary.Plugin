using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;

namespace MyLibrary.Plugin
{
    [Author(Name = "Karina")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Starting adding emplyees...");

            var employeesList = args.Cast<EmployeesDTO>().ToList();


            // Open the file to read from.
            string[] readText = File.ReadAllLines("Objects.txt");
            int count = readText.Count();
            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 0)
                {
                    employeesList.Add(new EmployeesDTO() { Name = readText[i] });
                }
                else
                {
                    employeesList.LastOrDefault().AddPhone(readText[i]);
                }
            }


            logger.Info("Employees added");

            logger.Info("Starting Viewer");
            logger.Info("Type q or quit to exit");
            logger.Info("Available commands: list, add, del");


            string command = "";

            while (!command.ToLower().Contains("quit"))
            {
                Console.Write("> ");
                command = Console.ReadLine();

                switch (command)
                {
                    case "list":
                        int index = 0;
                        foreach (var employee in employeesList)
                        {
                            Console.WriteLine($"{index} Name: {employee.Name} | Phone: {employee.Phone}");
                            ++index;
                        }
                        break;
                    case "add":
                        Console.Write("Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Phone: ");
                        string phone = Console.ReadLine();
                        Console.WriteLine($"{name} added to employees");
                        break;
                    case "del":
                        Console.Write("Index of employee to delete: ");
                        int indexToDelete;
                        if (!Int32.TryParse(Console.ReadLine(), out indexToDelete))
                        {
                            logger.Error("Not an index or not an int value!");
                        }
                        else
                        {
                            if (indexToDelete > 0 && indexToDelete < employeesList.Count())
                            {
                                employeesList.RemoveAt(indexToDelete);
                            }
                        }
                        break;
                }

                Console.WriteLine("");
            }

            return employeesList.Cast<DataTransferObject>();
        }
    }
}
