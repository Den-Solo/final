using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheVigenereCipher.Models
{

    public class InputDataModel 
    {
        public HttpPostedFileBase InputFile { get; set; }
        public string KeyWord { get; set; }
    }
}