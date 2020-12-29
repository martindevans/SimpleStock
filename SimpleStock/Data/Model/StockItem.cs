using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleStock.Extensions;

namespace SimpleStock.Data.Model
{
    [Table("StockItems")]
    public class StockItem
    {
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductNumber { get; set; }
        public string Manufacturer { get; set; }
        public string DatasheetUrl { get; set; }
    }
}
