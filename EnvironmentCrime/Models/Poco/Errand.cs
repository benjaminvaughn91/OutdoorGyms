using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class Errand
    {
        public int ErrandId { get; set; }

        public String RefNumber { get; set; }
        
        [Display(Name = "Var har brottet skett någonstans?")]
        [Required(ErrorMessage = "Du måste fylla i brottsplatsen.")]
        public String Place { get; set; }
        
        [Display(Name = "Vilken typ av brott?")]
        [Required(ErrorMessage = "Du måste fylla vilken typ brottet är av.")]
        public String TypeOfCrime { get; set; }
        
        [Display(Name = "När skedde brottet?")]
        [Required(ErrorMessage = "Du måste fylla i datumet för händelsen.")]
        [DisplayFormat(DataFormatString="{ 0:yyyy - MM - dd}")]
        public DateTime DateOfObservation { get; set; }

        public String Observation { get; set; }

        public String InvestigatorInfo { get; set; }

        public String InvestigatorAction { get; set; }
       
        [Display(Name = "Ditt namn (för- och efternamn):")]
        [Required(ErrorMessage = "Du måste fylla i ditt namn.")]
        public String InformerName { get; set; }
        
        [Display(Name = "Din telefon:")]
        [Required(ErrorMessage = "Du måste fylla i ditt telefonnummer.")]
        [RegularExpression(@"^\d{1,20}", ErrorMessage = "Inte korrekt ifyllt, använd siffror.")]
        public String InformerPhone { get; set; }
        
        public String StatusId { get; set; }
        
        public String DepartmentId { get; set; }
        
        public String EmployeeId { get; set; }

        public ICollection<Picture> Pictures { get; set; }

        public ICollection<Sample> Samples { get; set; }
    }
}
