using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleStock.Data.Model
{
    public class Transaction
    {
        [Key]
        public long ID { get; set; }

        [ForeignKey(nameof(StockItem.ID))]
        public long StockItemId { get; set; }

        public int Delta { get; set; }
        public string? Message { get; set; }

        public DateTime Date { get; set; }
    }
}
