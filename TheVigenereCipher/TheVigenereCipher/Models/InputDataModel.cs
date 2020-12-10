using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheVigenereCipher.Models
{

    public class InputDataModel 
    {
        public HttpPostedFileBase InputFile { get; set; }
        public string InputText { get; set; }
        public string KeyWord { get; set; }
        public bool ToEncrypt { get; set; } //or decrypt
        public bool FromFile { get; set; } //or from text

    }
}