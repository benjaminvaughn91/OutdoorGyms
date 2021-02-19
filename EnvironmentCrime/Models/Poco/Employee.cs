using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "Du måste fylla i användarnamn/ID")]
        public String EmployeeId { get; set; }

        [Required(ErrorMessage = "Du måste fylla i lösenordet")]
        [RegularExpression("[A-Za-z]{4}[0-9]{2}[?]", ErrorMessage = "Använd formatet BBBBSS? där B=bokstav och S=siffra. Ex: Pass00?")]
        public String EmployeePassword { get; set; }

        [Required(ErrorMessage = "Du måste fylla den anställdes namn.")]
        public String EmployeeName { get; set; }

        [Required(ErrorMessage = "Du måste välja den anställdes roll.")]
        public String RoleTitle { get; set; }

        [Required(ErrorMessage = "Du måste välja den anställdes avdelning.")]
        public String DepartmentId { get; set; }
    }
}
