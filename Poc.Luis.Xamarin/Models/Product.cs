using System;

namespace Poc.Luis.Xamarin
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime RecordedDate { get; set; }
    }
}