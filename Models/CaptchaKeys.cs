using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class CaptchaKeys
    {
        public CaptchaKeys()
        {

        }

        public CaptchaKeys(string key, string Account)
        {
            Key = key;
            this.Account = Account;
        }

        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Account { get; set; }
    }
}
