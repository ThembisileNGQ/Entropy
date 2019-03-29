// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer4.Quickstart.UI
{
    public class LoginInputModel
    {
        public string Username { get; set; } = "mrblue";
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public List<SelectListItem> Users { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "mrblack", Text = "mrblack" },
            new SelectListItem { Value = "mrpurple", Text = "mrpurple" },
            new SelectListItem { Value = "mrcyan", Text = "mrcyan"  },
            new SelectListItem { Value = "mrblue", Text = "mrblue"  },
            new SelectListItem { Value = "mrred", Text = "mrred"  },
        };
    }
}