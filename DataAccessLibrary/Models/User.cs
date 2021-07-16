
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLibrary.Models
{

    public class User : IdentityUser
    {
        public int BirthDate { get; set; }
        public string DeleteDate { get; set; }

    }
}
