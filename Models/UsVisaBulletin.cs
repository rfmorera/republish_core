using System;
using System.Collections.Generic;

namespace Republish.Models
{
    public class UsVisaBulletin
    {
        public int PreferenceCategoryId { get; set; }
        public string PreferenceType { get; set; }
        public int CountryId { get; set; }
        public DateTime CutOffDate { get; set; }
        public string CutOffDateType { get; set; }
        public int Id { get; set; }
        public string BulletinMonth { get; set; }
        public int? SetNumber { get; set; }
    }
}
