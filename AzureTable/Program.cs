using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace AzureTable
{
    class Program
    {
        static string connString = "DefaultEndpointsProtocol=https;AccountName=azfunctionblobstrg;AccountKey=m1n/Wdgl3ZwCl6Gl7vuaX6WenE1ipl1dAktQUYYXqXYY9e3bBDENbgE6MgD/r8Vt2bdCrGzshUw8Zq+4wm8Xlg==;";
        static string tableName = "mukeshemployeetbl";
        static async Task Main(string[] args)
        {
            //CloudTableClient cloudTableClient = new CloudTableClient()
            CloudTable cldTable = await CreateTable();
            //await RetrievingEntity(cldTable);
            //await InsertSingleEntity(cldTable);
            await DeletingEntity(cldTable);
            Console.ReadLine();
        }

        /// <summary>
        /// This function will create table 
        /// </summary>
        public async static Task<CloudTable> CreateTable()
        {
            CloudTable tableEmp = null;
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableEmp = tableClient.GetTableReference(tableName);
                await tableEmp.CreateIfNotExistsAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return tableEmp;


        }

        /// <summary>
        /// This Function will insert entity
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> InsertEntityInBatch(CloudTable cloudTable)
        {
            Employee employee1 = new Employee("Training", Guid.NewGuid().ToString()) { Email = "emp1@gmail.com", PhoneNumber = 123 };
            Employee employee2 = new Employee("Training", Guid.NewGuid().ToString()) { Email = "emp2@gmail.com", PhoneNumber = 123 };


            TableBatchOperation tablebatchOperations = new TableBatchOperation();
            tablebatchOperations.InsertOrReplace(employee1);
            tablebatchOperations.InsertOrReplace(employee2);
            await cloudTable.ExecuteBatchAsync(tablebatchOperations);

            return true;
        }

        /// <summary>
        /// This function will insert single entity
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <returns></returns>
        public async static Task<bool> InsertSingleEntity(CloudTable cloudTable)
        {
            Employee employee3 = new Employee("Development", Guid.NewGuid().ToString()) { Email = "emp3@gmail.com", PhoneNumber = 123 };

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(employee3);
                TableResult result = await cloudTable.ExecuteAsync(insertOrMergeOperation);
                Employee insertedEmployee = result.Result as Employee;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            return true;
        }

        /// <summary>
        /// This function will retrieve entity
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> RetrievingEntity(CloudTable cloudTable)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Employee>("Training", "b13cafd2-53f7-4580-a4ab-d8c27e021ab1");
                TableResult result = await cloudTable.ExecuteAsync(retrieveOperation);
                Employee employee = result.Result as Employee;
                if (employee != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", employee.PartitionKey, employee.RowKey, employee.Email, employee.PhoneNumber);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            return true;
        }

        /// <summary>
        /// This Function will delete an entity
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <returns></returns>
        public async static Task<bool> DeletingEntity(CloudTable cloudTable)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Employee>("Development", "16833382-2ca5-40d0-9b39-4b418da6b5e9");
            TableResult result = await cloudTable.ExecuteAsync(retrieveOperation);

            Employee employee = result.Result as Employee;
            if (employee != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(employee);
                TableResult res = await cloudTable.ExecuteAsync(deleteOperation);
            }

            return true;

        }
    }

    public class Employee : TableEntity
    {
        public string Email { get; set; }
        public int PhoneNumber { get; set; }

        public Employee()
        {

        }

        public Employee(string deptName, string empID)
        {
            this.PartitionKey = deptName;
            this.RowKey = empID;
        }
    }
}
