using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion_tax_calculator_net_core
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] SampleDates =
            {
                "2013-01-14 21:00:00",
                "2013-01-15 21:00:00",
                "2013-02-07 06:23:27",
                "2013-02-07 15:27:00",
                "2013-02-08 06:27:00",
                "2013-02-08 06:20:27",
                "2013-02-08 14:35:00",
                "2013-02-08 15:29:00",
                "2013-02-08 15:47:00",
                "2013-02-08 16:01:00",
                "2013-02-08 16:48:00",
                "2013-02-08 17:49:00",
                "2013-02-08 18:29:00",
                "2013-02-08 18:35:00",
                "2013-03-26 14:25:00",
                "2013-03-28 14:07:27"
            };

            List<DateTime> dates = new List<DateTime>();
            foreach (string s in SampleDates)
            {
                dates.Add(DateTime.Parse(s)); 
            }

            Vehicle car = new Vehicle("car", false);

            GothenburgTax gothenburg = new GothenburgTax(car, dates , 60 , 60);
            Console.WriteLine(gothenburg.GetTotalTollTaxAmount());  
            Console.ReadKey();
        }
    }
}
