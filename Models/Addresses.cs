using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Addresses
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Street { get; set; }
        public string Apt { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string ResFrom { get; set; }
        public string ResTo { get; set; }
        public int? Addrorder { get; set; }
        public int? Description { get; set; }
        public int? FirmId { get; set; }
        public string Fax { get; set; }
        public string Attnto { get; set; }
        public string Email { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? ResFromD { get; set; }
        public DateTime? ResToD { get; set; }
    }
}
