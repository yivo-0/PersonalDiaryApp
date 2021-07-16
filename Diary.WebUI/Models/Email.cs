using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diary.WebUI.Models
{
    public class Email
    {
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Message { get; set; }
    }
}
