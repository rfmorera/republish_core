using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Republish.Models
{
    public class LawFirm
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FirmId")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FirmName { get; set; }
        [Column("nContacts")]
        public int? NContacts { get; set; }
        [Column("nEmployers")]
        public int? NEmployers { get; set; }
        [Column("nCases")]
        public int? NCases { get; set; }
        [Column("logo")]
        [StringLength(150)]
        public string Logo { get; set; }
        [Column("immigration")]
        public int Immigration { get; set; }
        public int SessTimeOut { get; set; }
        [StringLength(100)]
        public string Webmail { get; set; }
        [StringLength(100)]
        public string Webcalendaring { get; set; }
        [StringLength(150)]
        public string Logoutlink { get; set; }
        [StringLength(250)]
        public string EmailBilling { get; set; }
        [StringLength(250)]
        public string EmailBillingcc { get; set; }
        [StringLength(50)]
        public string Billingcontact { get; set; }
        [Column("emaillawfirm")]
        [StringLength(100)]
        public string Emaillawfirm { get; set; }
        [Column("ClientLoginURL")]
        [StringLength(300)]
        public string ClientLoginUrl { get; set; }
        [Column("IntranetURL")]
        [StringLength(100)]
        public string IntranetUrl { get; set; }
        public bool Contactatty { get; set; }
        [Column("outboundemail")]
        [StringLength(150)]
        public string Outboundemail { get; set; }
        public bool? Autocasenumber { get; set; }
        [StringLength(150)]
        public string Logofirmemail { get; set; }
        [StringLength(150)]
        public string Website { get; set; }
        public bool UseOutBoundEmail { get; set; }
        [StringLength(8000)]
        public string QnrInstructions { get; set; }
        [StringLength(200)]
        public string Qnremaillawfirm { get; set; }
        public bool? Immigrationews { get; set; }
        [StringLength(150)]
        public string Supportemailcontact { get; set; }
        [StringLength(150)]
        public string Supportemailemployer { get; set; }
        [StringLength(50)]
        public string Liaison { get; set; }
        [StringLength(150)]
        public string Liaisonemail { get; set; }
        public bool? CustomReports { get; set; }
        public bool? FtpBackup { get; set; }
        [Column("FtpBackupURL")]
        [StringLength(150)]
        public string FtpBackupUrl { get; set; }
        [Column("RssURL")]
        [StringLength(250)]
        public string RssUrl { get; set; }
        [Required]
        [Column("formstoDB")]
        public bool? FormstoDb { get; set; }
        [Column("casenumformat")]
        [StringLength(150)]
        public string Casenumformat { get; set; }
        [Column("recptadvmd")]
        public bool? Recptadvmd { get; set; }
        [Column("maxusers")]
        public int Maxusers { get; set; }
        [Column("eimmigaccess")]
        public bool Eimmigaccess { get; set; }
        public bool? AutoNotify { get; set; }
        public bool? ResetPasswd { get; set; }
        public int? ResetPasswdays { get; set; }
        public bool? EnforcePasswdPolicy { get; set; }
        public bool? Enforceloginattmpt { get; set; }
        public int? Loginattmpts { get; set; }
        [Column("ShowPSS")]
        public int? ShowPss { get; set; }
        public int? Invoiceautonumstart { get; set; }
        [Column("FEIN")]
        [StringLength(50)]
        public string Fein { get; set; }
        public bool? LogoOrAddr { get; set; }
        [StringLength(150)]
        public string TimesTrackeremail { get; set; }
        [Column("EB5")]
        public bool Eb5 { get; set; }
        public bool Trustaccnt { get; set; }
        [Column("alertstatus")]
        public int? Alertstatus { get; set; }
        [Column("alertsubject")]
        [StringLength(50)]
        public string Alertsubject { get; set; }
        [Column("alertmssg")]
        [StringLength(2500)]
        public string Alertmssg { get; set; }
        [Column("alertdate", TypeName = "datetime")]
        public DateTime? Alertdate { get; set; }
        [Column("oldforms")]
        public bool? Oldforms { get; set; }
        [Column("alertlink")]
        [StringLength(250)]
        public string Alertlink { get; set; }
        [Column("fninstructions")]
        public string Fninstructions { get; set; }
        [Column("hrinstructions")]
        public string Hrinstructions { get; set; }
        [Column("qnrsubmitext")]
        public string Qnrsubmitext { get; set; }
        [Column("supportlicenses")]
        public int Supportlicenses { get; set; }
        [Column("ccfailed")]
        public int Ccfailed { get; set; }
        [Column("payment_link")]
        [StringLength(500)]
        public string PaymentLink { get; set; }
        [Column("caseautonumstart")]
        public int? Caseautonumstart { get; set; }
        [Column("smtp_external_use")]
        public bool ExternalUse { get; set; }
        [Column("smtp_external_name")]
        [StringLength(50)]
        public string ExternalName { get; set; }
        [Column("smtp_external_port")]
        public int ExternalPort { get; set; }
        [Column("smtp_external_timeout")]
        public int ExternalTimeout { get; set; }
        [Column("smtp_external_username")]
        [StringLength(50)]
        public string ExternalUsername { get; set; }
        [Column("smtp_external_password")]
        [StringLength(500)]
        public string ExternalPassword { get; set; }
        [Column("smtp_basicauth")]
        public int Basicauth { get; set; }
        [Required]
        [Column("contact_email_xfer2form")]
        public bool? ContactEmailXfer2form { get; set; }
        [Column("autoreminders_email")]
        [StringLength(200)]
        public string AutoremindersEmail { get; set; }
        [Column("contactnumberstart")]
        public int? Contactnumberstart { get; set; }
        [Column("enable_sso_login")]
        public bool EnableSsoLogin { get; set; }
        [Column("identity_provider_id")]
        [StringLength(64)]
        public string IdentityProviderId { get; set; }
        [Column("smtp_client_account")]
        public bool ClientAccount { get; set; }
        [Column("api_payments_enable")]
        public bool ApiPaymentsEnable { get; set; }
        [Column("api_clio_enable")]
        public bool? ApiClioEnable { get; set; }
        [Column("smtp_from_email")]
        [StringLength(100)]
        public string FromEmail { get; set; }
        [Column("pdf_editable")]
        public bool? PdfEditable { get; set; }
    }
}
