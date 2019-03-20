using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class Marriages
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Dom { get; set; }
        public string DomCity { get; set; }
        public string DomState { get; set; }
        public string DomCntry { get; set; }
        public string Dot { get; set; }
        public string DotCity { get; set; }
        public string DotState { get; set; }
        public string DotCntry { get; set; }
        public string SpsFirst { get; set; }
        public string SpsMiddle { get; set; }
        public string SpsLast { get; set; }
        public DateTime? DomD { get; set; }
        public DateTime? DotD { get; set; }
        public DateTime? SpsDob { get; set; }
        public string SpsSex { get; set; }
        public string SpsMaiden { get; set; }
        public bool? Curmar { get; set; }
        public int? Marorder { get; set; }
        public int? Spsuserid { get; set; }
        public int? Firmid { get; set; }
    }
}
