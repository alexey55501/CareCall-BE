using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPlatform.API.DAL.Models
{
    public class Request : RequestBase
    {
        public Request() { 
        }

        public virtual ApplicationUser Creator { get; set; }
    }
}
