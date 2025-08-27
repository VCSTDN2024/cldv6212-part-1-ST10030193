using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Customer: ITableEntity
    {
        public string PartitionKey { get; set; } = "customers";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Address { get; set; }

    }
}
