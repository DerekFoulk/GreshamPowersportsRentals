using BlazorApp.Shared;

namespace Data.Specialized.Models
{
    public class Pricing : Entity
    {
        public Pricing(decimal msrp, decimal price)
        {
            Msrp = msrp;
            Price = price;
        }

        public decimal Msrp { get; set; }
        public decimal Price { get; set; }
    }
}
