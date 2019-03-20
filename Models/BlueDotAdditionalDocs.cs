using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class RepublishAdditionalDocs
    {
        public int Sdid { get; set; }
        public int? CatalogId { get; set; }
        public int? UserId { get; set; }
        public string Expirationvar { get; set; }
        public DateTime? Expiration { get; set; }
        public string Idnumber { get; set; }
        public DateTime? Issuedate { get; set; }
        public bool? Docurrent { get; set; }
        public string Docsection { get; set; }
        public DateTime? Filedon { get; set; }
        public bool? Prioritydate { get; set; }
        public DateTime? Approvaldate { get; set; }
        public string Sponsor { get; set; }
        public int? Preferencetype { get; set; }
        public DateTime? Arrivaldate { get; set; }
        public string Arrivalcity { get; set; }
        public string Arrivalstate { get; set; }
        public DateTime? Departuredate { get; set; }
        public int? Firmid { get; set; }
        public DateTime? Fromdate { get; set; }
    }
}
