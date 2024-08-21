using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoriApp.DB.Models
{
    public class Usuarios
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } 
        public string UserDesc { get; set; }
        public string UserPic { get; set; }
    }
}
