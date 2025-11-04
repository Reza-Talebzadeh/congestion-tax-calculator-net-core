using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion_tax_calculator_net_core
{
    public interface IVehicle
    {
        bool IsFree { get; set; }
        string VehicleType { get; set; }
        string GetVehicleType();
        bool IsTollFree();
    }

    public class Vehicle : IVehicle
    {
        public bool IsFree { get ; set ; }
        public string VehicleType { get ; set ; }

        public Vehicle(string type, bool isFree)
        {
            VehicleType = type;
            IsFree = isFree;
        }

        public string GetVehicleType()
        {
            return VehicleType;
        }

        public bool IsTollFree()
        {
            return IsFree;
        }
    }

}
