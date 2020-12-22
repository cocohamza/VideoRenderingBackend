using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoRenderingBackend.Models
{
    public class VideoImport
    {

        public int storeId { get; set; }
        public string HeadText { get; set; }
        public string TailText { get; set; }
        public string Path { get; set; }
        public string CityText { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> OldPrices { get; set; }
        public List<string> NewPrices { get; set; }

        public List<NameUrlStructure> Images { get; set; }
    }

    public class NameUrlStructure
    {
        public string name { get; set; }
        public string url { get; set; }
    }
}
