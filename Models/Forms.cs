using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Forms
    {
        public string FormName { get; set; }
        public string FormDesc { get; set; }
        public int Qid { get; set; }
        public DateTime? Expdate { get; set; }
        public bool Flavor { get; set; }
        public bool Supplement { get; set; }
        public int AutoId { get; set; }

        public decimal? FilingFee { get; set; }
        public DateTime? Formver { get; set; }
        public bool BeingUpdated { get; set; }
        public DateTime? UpdateEta { get; set; }
        public bool IsObsolete { get; set; }
        public bool NewGovVersion { get; set; }
        public bool EnhancedVersion { get; set; }
        public bool ReceiptAble { get; set; }
        public int Fillable { get; set; }
        public DateTime? Datearchived { get; set; }
        public string Pdfurl { get; set; }
        
        

        public int? AltBtns { get; set; }
        public int? RsBeneficiary { get; set; }
        public int? RsBeneficiaryHistAddresses { get; set; }
        public int? RsBeneficiaryCurMarriage { get; set; }
        public int? RsBeneficiaryHistMarriages { get; set; }
        public int? RsBeneficiaryCurEmployment { get; set; }
        public int? RsBeneficiaryHistEmployment { get; set; }
        public int? RsBeneficiaryHistEducation { get; set; }
        public int? RsBeneficiaryHistTravel { get; set; }
        public int? RsBeneficiarySpouse { get; set; }
        public int? RsBeneficiaryFather { get; set; }
        public int? RsBeneficiaryMother { get; set; }
        public int? RsBeneficiaryKids { get; set; }
        public int? RsBeneficiarySiblings { get; set; }
        public int? RsContact { get; set; }
        public int? RsEmployer { get; set; }
        public int? RsEmployerAddress { get; set; }
        public int? RsEmployerSignatory { get; set; }
        public int? RsPetitioner { get; set; }
        public int? RsPetitionerHistAddresses { get; set; }
        public int? RsPetitionerCurMarriage { get; set; }
        public int? RsPetitionerHistMarriages { get; set; }
        public int? RsPetitionerCurEmployment { get; set; }
        public int? RsPetitionerHistEmployment { get; set; }
        public int? RsPetitionerFather { get; set; }
        public int? RsPetitionerMother { get; set; }
    }
}
