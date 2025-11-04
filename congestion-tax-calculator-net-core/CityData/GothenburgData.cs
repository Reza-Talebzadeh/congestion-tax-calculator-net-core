using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion_tax_calculator_net_core.TollRules
{
    public class GothenburgData
    {
        public static List<TollFeePeriod> TollPeriods => tollPeriods;

        static List<TollFeePeriod> tollPeriods = new List<TollFeePeriod>
        {
            new TollFeePeriod("06:00", "06:29", 8),
            new TollFeePeriod("06:30", "06:59", 13),
            new TollFeePeriod("07:00", "07:59", 18),
            new TollFeePeriod("08:00", "08:29", 13),
            new TollFeePeriod("08:30", "14:59", 8),
            new TollFeePeriod("15:00", "15:29", 13),
            new TollFeePeriod("15:30", "16:59", 18),
            new TollFeePeriod("17:00", "17:59", 13),
            new TollFeePeriod("18:00", "18:29", 8),
            new TollFeePeriod("18:30", "05:59", 0)
        };

        public static Dictionary<int, List<int>> TollFreeDaysByMonth => tollFreeDaysByMonth;

        static readonly Dictionary<int, List<int>> tollFreeDaysByMonth = new Dictionary<int, List<int>>()
        {
            { 1, new List<int> { 1 } },
            { 3, new List<int> { 28, 29 } },
            { 4, new List<int> { 1, 30 } },
            { 5, new List<int> { 1, 8, 9 } },
            { 6, new List<int> { 5, 6, 21 } },
            { 7, null }, 
            { 11, new List<int> { 1 } },
            { 12, new List<int> { 24, 25, 26, 31 } }
        };

    }
}
