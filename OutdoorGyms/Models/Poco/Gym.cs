using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OutdoorGyms.Models
{
    public class Gym
    {
        public int GymId { get; set; }

        public String RefNumber { get; set; }

        [Display(Name = "What town is the gym located in?")]
        [Required(ErrorMessage = "You need to write the town of the gym.")]
        public String Town { get; set; }

        [Display(Name = "What is the name or location of the gym?")]
        [Required(ErrorMessage = "You need to write the location of the gym.")]
        public String Place { get; set; }
        
        public DateTime DateOfContribution { get; set; }

        public String Description { get; set; }
       
        [Display(Name = "What is your name?:")]
        [Required(ErrorMessage = "You need to enter your name.")]
        public String ContributorName { get; set; }
        
        public String StatusId { get; set; }
        
        public String CountyId { get; set; }
        
        public String EmployeeId { get; set; }

        public ICollection<Picture> Pictures { get; set; }
    }
}
