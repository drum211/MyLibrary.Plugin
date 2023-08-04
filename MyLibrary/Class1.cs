using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
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
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            logger.Info("Starting adding emplyees...");

            var employeesList = args.Cast<EmployeesDTO>().ToList();


            using (var http = new HttpClient())
            {
                var endpoint = "https://dummyjson.com/users";
                var result = http.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                var objRoot = JsonConvert.DeserializeObject<Root>(json);
                



                int count = objRoot.users.Count;
                for (int i = 0; i < count; i++)
                {
                    employeesList.Add(new EmployeesDTO() { Name = $"{objRoot.users[i].firstName} {objRoot.users[i].lastName}" });
                    employeesList.LastOrDefault().AddPhone(objRoot.users[i].phone);
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
                        string str2 = Console.ReadLine();
                        Console.Write("Phone: ");
                        string phone = Console.ReadLine();
                        string warningText = "cannot be an empty value! The object wasn't added to data base. Please, check the value and try again";
                        if (str2 == null || str2 == "")
                        {
                            Console.WriteLine($"Name {warningText}");
                        }
                        if (phone == null || phone == "")
                        {
                            Console.WriteLine($"Phone {warningText}");
                        }
                        else
                        {
                            employeesList.Add(new EmployeesDTO() { Name = str2 });
                            employeesList.LastOrDefault().AddPhone(phone);
                            Console.WriteLine(str2 + " added to employees");
                        }
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
                            if (indexToDelete >= 0 && indexToDelete < employeesList.Count())
                            {
                                employeesList.RemoveAt(indexToDelete);
                                logger.Error($"Employee at index {indexToDelete} was deleted successfully!");
                            }
                        }
                        break;
                }

                Console.WriteLine("");
            }

            return employeesList.Cast<DataTransferObject>();
        }
    }

    public class Root
    {
        public List<User> users { get; set; }
        public int total { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
    }

    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
    }
}
