using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace EFApp
{
    [Table("Contacts")]
    public class Person
    {
        [PrimaryKey]
        public string email { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string public_key { get; set; }
    }
}
