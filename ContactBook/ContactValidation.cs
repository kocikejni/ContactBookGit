using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContactBook
{
    public class ContactValidation
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter first name", AllowEmptyStrings = false)]
        public string ContactFirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string ContactLastName { get; set; }
        
        [Display(Name = "Contact Number 1")]
        [Required(ErrorMessage = "Please enter a number", AllowEmptyStrings = false)]
        public string ContactNo1 { get; set; }
        
        [Display(Name = "Contact Number 2")]
        public string ContactNo2 { get; set; }
        
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "Email not valid")]
        public string Email { get; set; }
        
        [Display(Name = "Country ID")]
        public int CountryID { get; set; }
        
        [Display(Name = "Adress")]
        public string Adress { get; set; }
    }

    [MetadataType(typeof(ContactValidation))] //Ben Vertetimin

    public partial class Contact
    {

    }
}