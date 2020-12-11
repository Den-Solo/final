using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;


namespace Cipher.Models
{
    public class InputDataModel
    {
        public HttpFile InputFile { get; set; }
        public string InputText { get; set; }
        public string KeyWord { get; set; }
        public bool ToEncrypt { get; set; } //or decrypt
        public bool FromFile { get; set; }  //or from rawtext

    }
}