using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Employment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmpName { get; set; }
        public string EmpStreet { get; set; }
        public string EmpSuite { get; set; }
        public string EmpCity { get; set; }
        public string EmpState { get; set; }
        public string EmpZip { get; set; }
        public string EmpCntry { get; set; }
        public string EmpPh { get; set; }
        public string EmpFax { get; set; }
        public string EmpEmail { get; set; }
        public string EmpStart { get; set; }
        public string EmpTill { get; set; }
        public string JobTitle { get; set; }
        public string JobDesc { get; set; }
        public DateTime? EmpStartD { get; set; }
        public DateTime? EmpTillD { get; set; }
        public bool? CurEmp { get; set; }
        public int? Emporder { get; set; }
        public int? FirmId { get; set; }
        public int? HoursWeek { get; set; }
        public string KindBusiness { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorPhone { get; set; }
    }
}
