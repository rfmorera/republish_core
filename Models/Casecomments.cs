using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Casecomments
    {
        public int Comid { get; set; }
        public decimal? CaseId { get; set; }
        public bool? Visible { get; set; }
        public string Comments { get; set; }
        public decimal? Updatedby { get; set; }
        public DateTime? Lastupdate { get; set; }
        public bool? CommBy { get; set; }
        public string SectionCase { get; set; }
        public int? FirmId { get; set; }
        public int? Importance { get; set; }
        public bool? VisibleEmployer { get; set; }
        public string SectionName { get; set; }
        public string Tags { get; set; }
        public bool? IsMessage { get; set; }
        public int? ParentMessageid { get; set; }
        public int? SemParentRecipientRead { get; set; }
        public int? SemParentSenderRead { get; set; }
        public string SemSubject { get; set; }
        public int? SemSenderId { get; set; }
        public int? SemRecipientId { get; set; }
    }
}
