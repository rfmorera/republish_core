using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Results
{
    public class InsertResult
    {
        //public InsertResult(string id, string token)
        //{
        //    Id = id;
        //    Token = token;
        //}
        public InsertResult(string full)
        {
            FullId = full;
        }
        //public string Id { get; set; }
        //public string Token { get; set; }
        public string FullId
        {
            get;
            set;
        }
    }
}
