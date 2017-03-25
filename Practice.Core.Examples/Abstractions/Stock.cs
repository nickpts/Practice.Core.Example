using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Core.Examples.Abstractions
{
    public class Stock
    {
        string symbol;
        decimal price;

        public Stock(string symbol)
        {
            this.symbol = symbol;
            PriceChanged += ReportChange; // connect event 
        }

        public event PriceChangedHandler PriceChanged;

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price == value) return;
                decimal oldPrice = price;
                price = value;
                PriceChanged?.Invoke(oldPrice, price);

                var t = new Tuple<int, string>(123, "hello");

            }
        }

        public void ReportChange(decimal oldPrice, decimal newPrice) => Console.WriteLine($"Old price: {oldPrice}, newPrice : {newPrice}");
    }
}
