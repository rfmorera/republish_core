using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Partiescont
    {
        public int PartiescontId { get; set; }
        public int PartcontId { get; set; }
        public int FirmId { get; set; }
        public int UserId { get; set; }
        public string Relation { get; set; }
    }
}
