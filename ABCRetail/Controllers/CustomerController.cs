using System.Collections.Concurrent;
using System.Threading.Tasks;
using ABCRetail.Models;
using ABCRetail.Services;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class CustomerController : Controller
    {
        EntityService _customerService;
        private readonly string tableName = "Customers";
        public CustomerController(EntityService customerService) 
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            try { 

            //list that will store the info retrieved from the Azure Table
            var customers = new List<Customer>();

            await foreach (var c in _customerService.GetAllEntityAsync<Customer>(tableName))
            {
                customers.Add(c);
            }

            return View(customers);
            }
            catch (Exception) 
            {
                return View("Error");
            }
        }

        //Insert 
        [HttpGet]
        public IActionResult Create()
        {
            return  View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer entity) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(entity);
                }

                var createdEntity = await _customerService.InsertUpdateAsync<Customer>(entity,tableName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("COULD NOT ADD: " + ex.Message);
                return View("Error");
            }
        }

        //Update
        [HttpGet]
        public async  Task<IActionResult> Update(string PartitionKey, string RowKey)
        {
            //Get entity that has to updated
            var c = await _customerService.GetEntityAsync<Customer>(tableName,PartitionKey, RowKey);

            if(c == null)
            {
               return RedirectToAction("Index");
            }

            return View(c);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Customer entity)
        {
            if (!ModelState.IsValid) 
            {
                return View(entity);
            }

            try
            {
                await _customerService.InsertUpdateAsync<Customer>(entity, tableName);
                return RedirectToAction("Index");
            }
            catch(Exception ex) 
            {
                Console.WriteLine("COULD NOT ADD: " + ex.Message);
                return View("Error");
            }
           
        }

       
        [HttpGet]
        public async Task<IActionResult> Delete(string PartitionKey, string RowKey)
        {
            var c = await _customerService.GetEntityAsync<Customer>(tableName,PartitionKey, RowKey);

            if (c == null)
            {
                return RedirectToAction("Index");
            }

            return View(c);
        }

        // POST: Perform the delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string PartitionKey, string RowKey)
        {
            await _customerService.DeleteEntityAsync(tableName,PartitionKey, RowKey);
            return RedirectToAction("Index");
        }


        //search and view
        [HttpGet]
        [ActionName(nameof(SearchAsync))]
        public async Task<IActionResult> SearchAsync([FromQuery] string category, string id)
        {
            return Ok(await _customerService.GetEntityAsync<Customer>(tableName,category, id));
        }


    }
}
