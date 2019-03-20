using System;
using System.Collections.Generic;

namespace Republish.Data
{
    public class FormsMapping
    {
        public int MapId { get; set; }
        public string UserType { get; set; }
        public string TblFld { get; set; }
        public string FormFld { get; set; }
        public string Qffield { get; set; }
        public string Qfrsvalue { get; set; }
        public string RepFld { get; set; }
        public string Qfrole { get; set; }
        public string Qffldproperties { get; set; }
        public string Comment { get; set; }
        public string SaveInst { get; set; }
        public int? Xfertoform { get; set; }
        public string FieldType { get; set; }
        public int? FieldLength { get; set; }
        public bool IsRequired { get; set; }
        public int IsCalculated { get; set; }
    }
}
