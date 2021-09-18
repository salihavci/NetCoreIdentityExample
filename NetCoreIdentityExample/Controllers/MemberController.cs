using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Controllers
{
    [Authorize] //Bu controller'a sadece üyeler girebilir.
    public class MemberController : Controller
    {
        //[Authorize] //Bu sayfaya sadece üyeler girebilir.
        public IActionResult Index()
        {
            return View();
        }
    }
}
