using System;
using System.Collections.Generic;
using System.Text;

namespace Republish.Extensions
{
    public static class StringExtension
    {
        public static string ToGenderString(this string sex)
        {
            switch (sex)
            {
                case "2":
                    return "Female";
                default:
                    return "Male";
            }
        }

        public static string ToMaritalStatusString(this string maritalStatus)
        {
            switch (maritalStatus)
            {
                case "2":
                    return "Married";
                case "3":
                    return "Divorced";
                case "4":
                    return "Legally Separated";
                case "5":
                    return "Widowed";
                case "8":
                    return "Marriage Annulled";
                default:
                    return "Single";
            }
        }
    }
}
