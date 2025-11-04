using congestion_tax_calculator_net_core.TollRules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion_tax_calculator_net_core
{
    internal class GothenburgTax : BaseTollCalc
    {

        public override void CalcTollAmnt()
        {
            SetRules( GothenburgData.TollPeriods , GothenburgData.TollFreeDaysByMonth);
            base.CalcTollAmnt();
            
        }

        public GothenburgTax(Vehicle vehicle,List<DateTime> dates ,double maxTaxAmntPerDay , int scopeFreeToll)
        {
            MaxTaxAmntPerDay = maxTaxAmntPerDay;
            ScopeFreeToll = scopeFreeToll;
            InputVehicle = vehicle;
            InputDates = dates.OrderBy(d=>d).ToList();

            CalcTollAmnt();

        }
    }
}
