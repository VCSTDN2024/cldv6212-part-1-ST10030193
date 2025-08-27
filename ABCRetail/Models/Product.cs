using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Product : ITableEntity
    {
        public string PartitionKey { get; set; } = "product";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        [Required]
        public string? ProductName { get; set; }

        [Required]
        public string? ProductDescription { get; set; }

        [Required]
        public string? ProductType { get; set; }

        public string? ProductImageURL { get; set; }


    }
}
