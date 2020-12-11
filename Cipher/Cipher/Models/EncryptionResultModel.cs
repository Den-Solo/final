using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cipher.Models
{
    public class EncryptionResultModel
    {
        public bool IsError { get; set; }
        public string Content { get; set; } //either cipthertext or error msg
    }
}