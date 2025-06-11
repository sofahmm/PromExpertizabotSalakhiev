using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertizaWPF.DataBase
{
    public class Application
    {
        public int Id { get; set; }
        public string? User { get; set; }
        public string? Executor { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Date_create { get; set; }
        public string? Date_close { get; set; }
        public string? Object_Id { get; set; }
    }
}
