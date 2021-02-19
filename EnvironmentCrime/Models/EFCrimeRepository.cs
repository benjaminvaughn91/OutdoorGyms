using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class EFCrimeRepository : ICrimeRepository
    {
        private ApplicationDbContext context;
        private IHttpContextAccessor contextAcc;

        public EFCrimeRepository(ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            context = ctx;
            contextAcc = cont;
        }

        public IQueryable<Errand> Errands => context.Errands.Include(e=>e.Pictures).Include(e=>e.Samples);

        public IQueryable<ErrandStatus> ErrandStatuses => context.ErrandStatuses;

        public IQueryable<Department> Departments => context.Departments;

        public IQueryable<Employee> Employees => context.Employees;

        public IQueryable<Picture> Pictures => context.Pictures;

        public IQueryable<Sample> Samples => context.Samples;

        public IQueryable<Sequence> Sequences => context.Sequences;

        /*Saves a new Errand to the database. A reference number will be generated and include the CurrentValue from
         * the Sequences-table, which will then be increased by one.*/
        public string SaveErrand(Errand errand)
        {
            string refNr = "";
            if (errand.ErrandId == 0) 
            {
                var sequence = Sequences.Where(td => td.Id.Equals(1)).FirstOrDefault();
                int currentValue = sequence.CurrentValue;
                refNr = "2020-45-" + currentValue;
                errand.RefNumber = refNr;
                errand.StatusId = "S_A";
                context.Errands.Add(errand);
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

        /*Saves a Sample to the Sample table.*/
        public void SaveSample(Sample sample)
        {
            context.Samples.Add(sample);
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

        /*Takes an Errand as parameter and finds the matching database entry in the Errands table. 
         * Updates some of the properties to match the parameter-Errand. */
        public void UpdateErrand(Errand errand)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(te => te.ErrandId == errand.ErrandId);
            if (dbEntry != null)
            {
                dbEntry.DepartmentId = errand.DepartmentId;
                dbEntry.EmployeeId = errand.EmployeeId;
                dbEntry.StatusId = errand.StatusId;
                dbEntry.InvestigatorInfo = errand.InvestigatorInfo;
                dbEntry.InvestigatorAction = errand.InvestigatorAction;
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
                dbEntry.DepartmentId = employee.DepartmentId;
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

        /*Return the Errand object with the same ErrandId as the specified id.*/
        public Errand GetErrandDetail(int id)
        {
            var errandDetails = Errands.Where(td => td.ErrandId.Equals(id)).FirstOrDefault();
            return errandDetails;
        }

        /*Returns the name of the department belonging to a specified Errand.*/
        public string GetErrandDepartment(Errand errand)
        {
            var departmentName = "";
            var obj = Departments.Where(td => td.DepartmentId.Equals(errand.DepartmentId)).FirstOrDefault();
            if (obj != null)
            {
                departmentName = obj.DepartmentName;
            }
            return departmentName;
        }

        /*Returns the name of the status belonging to a specified Errand.*/
        public string GetErrandStatus(Errand errand)
        {
            var statusName = "";
            var obj = ErrandStatuses.Where(td => td.StatusId.Equals(errand.StatusId)).FirstOrDefault();
            if (obj != null)
            {
                statusName = obj.StatusName;
            }
            return statusName;
        }

        /*Returns the name of the employee belonging to a specified Errand.*/
        public string GetErrandEmployee(Errand errand)
        {
            var employeeName = "";
            var obj = Employees.Where(td => td.EmployeeId.Equals(errand.EmployeeId)).FirstOrDefault();
            if (obj != null)
            {
                employeeName = obj.EmployeeName;
            }
            return employeeName;
        }

        /*Returns the name of the department belonging to a specified Employee.*/
        public string GetEmployeeDepartment(Employee employee)
        {
            var departmentName = "";
            var obj = Departments.Where(td => td.DepartmentId.Equals(employee.DepartmentId)).FirstOrDefault();
            if (obj != null)
            {
                departmentName = obj.DepartmentName;
            }
            return departmentName;
        }
    }
}
