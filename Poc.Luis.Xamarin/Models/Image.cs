using System;
namespace Poc.Luis.Xamarin
{
    public class Image : BaseEntity
    {
        public DateTime RecordedDate { get; set; }
        public string ImageBase64 { get; set; }
    }
}