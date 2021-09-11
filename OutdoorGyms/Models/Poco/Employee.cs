using System;
using System.ComponentModel.DataAnnotations;

namespace OutdoorGyms.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "You need to choose username/ID")]
        public String EmployeeId { get; set; }

        [Required(ErrorMessage = "You need to enter a password")]
        [RegularExpression("[A-Za-z]{4}[0-9]{2}[?]", ErrorMessage = "Använd formatet BBBBSS? där B=bokstav och S=siffra. Ex: Pass00?")]
        public String EmployeePassword { get; set; }

        [Required(ErrorMessage = "You need to enter employee name.")]
        public String EmployeeName { get; set; }

        [Required(ErrorMessage = "You need to set employee title.")]
        public String RoleTitle { get; set; }

        [Required(ErrorMessage = "You need to set employee county.")]
        public String CountyId { get; set; }
    }
}
