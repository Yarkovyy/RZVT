using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Gallery.DataAccess.Models
{
    public class User:IdentityUser<int>
    {
        public ICollection<Image> Images { get; set; } = null!;
    }
}
