using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutdoorGyms.Models
{
    public interface IGymRepository
    {
        IQueryable<Gym> Gyms { get; }

        IQueryable<County> Countys { get; }

        IQueryable<GymStatus> GymStatuses { get; }

        IQueryable<Employee> Employees { get; }

        IQueryable<Picture> Pictures { get; }

        IQueryable<Sequence> Sequences { get; }

        string SaveGym(Gym gym);

        void SavePicture(Picture picture);

        bool SaveEmployee(Employee employee);

        void UpdateGym(Gym gym);

        void UpdateEmployee(Employee employee);

        void RemoveEmployee(string employeeId);

        Employee GetCurrentEmployee();

        public string GetCurrentEmployeeID();

        Gym GetGymDetail(int id);

        string GetGymCounty(Gym gym);

        string GetGymStatus(Gym gym);

        string GetGymEmployee(Gym gym);

        string GetEmployeeCounty(Employee employee);
    }
}
