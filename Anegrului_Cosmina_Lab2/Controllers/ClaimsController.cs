using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anegrului_Cosmina_Lab2.Controllers
{
    public class ClaimsController : Controller
    {
        public ViewResult Index() => View(User?.Claims);
    }
}
