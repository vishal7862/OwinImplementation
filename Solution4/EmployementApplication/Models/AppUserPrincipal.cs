﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace EmployementApplication.Models
{
    public class AppUserPrincipal : ClaimsPrincipal
    {
        public AppUserPrincipal(ClaimsPrincipal principal)
            : base(principal)
        {
        }
        public string Name
        {

            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }
     
    }
}