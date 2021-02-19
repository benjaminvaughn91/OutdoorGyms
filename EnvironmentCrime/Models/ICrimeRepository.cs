using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public interface ICrimeRepository
    {
        IQueryable<Errand> Errands { get; }

        IQueryable<Department> Departments { get; }

        IQueryable<ErrandStatus> ErrandStatuses { get; }

        IQueryable<Employee> Employees { get; }

        IQueryable<Picture> Pictures { get; }

        IQueryable<Sample> Samples { get; }

        IQueryable<Sequence> Sequences { get; }

        string SaveErrand(Errand errand);

        void SavePicture(Picture picture);

        void SaveSample(Sample sample);

        bool SaveEmployee(Employee employee);

        void UpdateErrand(Errand errand);

        void UpdateEmployee(Employee employee);

        void RemoveEmployee(string employeeId);

        Employee GetCurrentEmployee();

        Errand GetErrandDetail(int id);

        string GetErrandDepartment(Errand errand);

        string GetErrandStatus(Errand errand);

        string GetErrandEmployee(Errand errand);

        string GetEmployeeDepartment(Employee employee);
    }
}
