using System;
using System.Collections.Generic;


namespace CarInventory.Models
{
    public partial class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool New { get; set; }
    }
}
