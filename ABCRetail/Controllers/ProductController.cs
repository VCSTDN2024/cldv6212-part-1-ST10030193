using System;
using ABCRetail.Models;
using ABCRetail.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProductController : Controller
    {
        private readonly EntityService _productService;
        private readonly string tableName = "Product";
        private readonly BlobServiceClient _blobServiceClient;
        public ProductController(EntityService productService, BlobServiceClient blobServiceClient)
        {
            _productService = productService;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                //list that will store the info retrieved from the Azure Table
                var products = new List<Product>();

                await foreach (var c in _productService.GetAllEntityAsync<Product>(tableName))
                {
                    products.Add(c);
                }

                return View(products);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //CREATE
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product entity, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model is not valid");
                return View(entity);
            }

            try
            {
                Console.WriteLine("Post has enterd and model is fine!!");

                if (imageFile != null && imageFile.Length > 0)
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient("productimage");
                    containerClient.CreateIfNotExists();

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        blobClient.Upload(stream);
                    }

                    entity.ProductImageURL = blobClient.Uri.ToString();
                }

                var createdEntity = await _productService.InsertUpdateAsync<Product>(entity, tableName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("COULD NOT ADD: " + ex.Message);
                return View();
            }
        }

        //Update
        [HttpGet]
        public async Task<IActionResult> Update(string PartitionKey, string RowKey)
        {
            //Get entity that has to updated
            var c = await _productService.GetEntityAsync<Product>(tableName, PartitionKey, RowKey);

            if (c == null)
            {
                return RedirectToAction("Index");
            }

            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Product entity, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient("ProductImage");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        blobClient.Upload(stream);
                    }

                    entity.ProductImageURL = blobClient.Uri.ToString();
                }
                await _productService.InsertUpdateAsync<Product>(entity, tableName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("COULD NOT ADD: " + ex.Message);
                return View("Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(string PartitionKey, string RowKey)
        {
            var c = await _productService.GetEntityAsync<Product>(tableName, PartitionKey, RowKey);

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
            await _productService.DeleteEntityAsync(tableName, PartitionKey, RowKey);
            return RedirectToAction("Index");
        }


        //search and view
        [HttpGet]
        [ActionName(nameof(SearchAsync))]
        public async Task<IActionResult> SearchAsync([FromQuery] string category, string id)
        {
            return Ok(await _productService.GetEntityAsync<Customer>(tableName, category, id));
        }
    }
}
