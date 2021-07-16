using Diary.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diary.WebUI.Services
{
    public interface IEmailService
    {
        public Task Send(Email email);
    }
}
