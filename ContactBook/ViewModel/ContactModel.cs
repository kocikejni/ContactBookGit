using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactBook.ViewModel
{
    public class ContactModel
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Adress { get; set; }
    }
}