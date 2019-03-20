using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Processcatalog
    {
        public int ProcesscatalogId { get; set; }
        public string ProcesscatalogVal { get; set; }
        public string CatDescription { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public int? CatalogFirmId { get; set; }
        public int? NoOrder { get; set; }
        public int? Status { get; set; }
        public bool? Uscislink { get; set; }
        public int? Qid { get; set; }
    }
}
