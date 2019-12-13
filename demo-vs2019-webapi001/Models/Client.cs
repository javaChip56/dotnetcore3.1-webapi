using System;
using System.Collections.Generic;

namespace demo_vs2019_webapi001.Models
{
    public partial class Client
    {
        public int Id { get; set; }
        public string Clientname { get; set; }
        public string Clientno { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Createdby { get; set; }
        public DateTime? Createddate { get; set; }
        public string Updatedby { get; set; }
        public DateTime? Updateddate { get; set; }
    }
}
