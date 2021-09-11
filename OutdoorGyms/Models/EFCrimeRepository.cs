using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OutdoorGyms.Models
{
    public class EFGymRepository : IGymRepository
    {
        private ApplicationDbContext context;
        private IHttpContextAccessor contextAcc;

        public EFGymRepository(ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            context = ctx;
            contextAcc = cont;
        }

        public IQueryable<Gym> Gyms => context.Gyms.Include(e=>e.Pictures);

        public IQueryable<GymStatus> GymStatuses => context.GymStatuses;

        public IQueryable<County> Countys => context.Countys;

        public IQueryable<Employee> Employees => context.Employees;

        public IQueryable<Picture> Pictures => context.Pictures;

        public IQueryable<Sequence> Sequences => context.Sequences;

        /*Saves a new Gym to the database. A reference number will be generated and include the CurrentValue from
         * the Sequences-table, which will then be increased by one.*/
        public string SaveGym(Gym gym)
        {
            string refNr = "";
            if (gym.GymId == 0) 
            {
                var sequence = Sequences.Where(td => td.Id.Equals(1)).FirstOrDefault();
                int currentValue = sequence.CurrentValue;
                refNr = "2020-45-" + currentValue;
                gym.RefNumber = refNr;
                gym.StatusId = "S_A";
                context.Gyms.Add(gym);
                sequence.CurrentValue++;
                context.SaveChanges();
            }
            return refNr;
        }

        /*Saves a Picture to the Picture table.*/
        public void SavePicture(Picture picture)
        {
            context.Pictures.Add(picture);
            context.SaveChanges();
        }

        /*Saves a new Employee to the database*/
        public bool SaveEmployee(Employee employee)
        {
            if (!(context.Employees.Any(te => te.EmployeeId == employee.EmployeeId)))
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                return true;
            }
            else return false;
        }

        /*Takes an Gym as parameter and finds the matching database entry in the Gyms table. 
         * Updates some of the properties to match the parameter-Gym. */
        public void UpdateGym(Gym gym)
        {
            Gym dbEntry = context.Gyms.FirstOrDefault(te => te.GymId == gym.GymId);
            if (dbEntry != null)
            {
                dbEntry.CountyId = gym.CountyId;
                dbEntry.EmployeeId = gym.EmployeeId;
                dbEntry.Description = gym.Description;
                dbEntry.StatusId = gym.StatusId;
                context.SaveChanges();
            }
        }

        /*Takes an Employee as parameter and finds the matching database entry in the Employee table. 
        * Updates some of the properties to match the parameter-Employee. */
        public void UpdateEmployee(Employee employee)
        {
            Employee dbEntry = context.Employees.FirstOrDefault(te => te.EmployeeId == employee.EmployeeId);
            if (dbEntry != null)
            {
                dbEntry.EmployeeName = employee.EmployeeName;
                dbEntry.EmployeePassword = employee.EmployeePassword;
                dbEntry.RoleTitle = employee.RoleTitle;
                dbEntry.CountyId = employee.CountyId;
                context.SaveChanges();
            }
        }

        /*Removes the Employee object with the specified ID from the database.*/
        public void RemoveEmployee(string employeeId)
        {
            Employee dbEntry = context.Employees.Where(te => te.EmployeeId == employeeId).FirstOrDefault();
            if (dbEntry != null)
            {
                context.Employees.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        /*Returns the Employee object who's Name matches the username of the currently logged in user. */
        public Employee GetCurrentEmployee()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var employee = Employees.Where(td => td.EmployeeId.Equals(userName)).FirstOrDefault();
            return employee; 
        }

        /*Returns the currently logged in users ID. */
        public string GetCurrentEmployeeID()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            return userName;
        }

        /*Return the Gym object with the same GymId as the specified id.*/
        public Gym GetGymDetail(int id)
        {
            var gymDetails = Gyms.Where(td => td.GymId.Equals(id)).FirstOrDefault();
            return gymDetails;
        }

        /*Returns the name of the county belonging to a specified Gym.*/
        public string GetGymCounty(Gym gym)
        {
            var countyName = "";
            var obj = Countys.Where(td => td.CountyId.Equals(gym.CountyId)).FirstOrDefault();
            if (obj != null)
            {
                countyName = obj.CountyName;
            }
            return countyName;
        }

        /*Returns the name of the status belonging to a specified Gym.*/
        public string GetGymStatus(Gym gym)
        {
            var statusName = "";
            var obj = GymStatuses.Where(td => td.StatusId.Equals(gym.StatusId)).FirstOrDefault();
            if (obj != null)
            {
                statusName = obj.StatusName;
            }
            return statusName;
        }

        /*Returns the name of the employee belonging to a specified Gym.*/
        public string GetGymEmployee(Gym gym)
        {
            var employeeName = "";
            var obj = Employees.Where(td => td.EmployeeId.Equals(gym.EmployeeId)).FirstOrDefault();
            if (obj != null)
            {
                employeeName = obj.EmployeeName;
            }
            return employeeName;
        }

        /*Returns the name of the county belonging to a specified Employee.*/
        public string GetEmployeeCounty(Employee employee)
        {
            var countyName = "";
            var obj = Countys.Where(td => td.CountyId.Equals(employee.CountyId)).FirstOrDefault();
            if (obj != null)
            {
                countyName = obj.CountyName;
            }
            return countyName;
        }
    }
}
