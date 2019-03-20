using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class PreferenceTypeModel
    {
        public int PrefTypeid { get; set; }
        public string PreferenceType { get; set; }
        public int FirmId { get; set; }
        public int? Ordernum { get; set; }
        public int? VisaBulletinId { get; set; }
    }
}
