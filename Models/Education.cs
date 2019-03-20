using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Education
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SchoolName { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Degree { get; set; }
        public string Major { get; set; }
        public DateTime? Receivedon { get; set; }
        public bool? Evaluated { get; set; }
        public int? Schoolorder { get; set; }
        public int FirmId { get; set; }
        public string City { get; set; }
        public DateTime? Attfrom { get; set; }
        public DateTime? Attuntil { get; set; }
    }
}
