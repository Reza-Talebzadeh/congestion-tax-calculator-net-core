

using System;

namespace congestion_tax_calculator_net_core.TollRules
{
    public class TollFeePeriod
    {


        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public double Fee { get; set; }

        public TollFeePeriod(string start, string end, int amount)
        {
            StartTime = TimeSpan.Parse(start);
            EndTime = TimeSpan.Parse(end);
            Fee = amount;
        }


    }
}