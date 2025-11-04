using System.Collections.Generic;
using System;
using congestion_tax_calculator_net_core.TollRules;
using System.Data;
using System.Linq;

namespace congestion_tax_calculator_net_core
{
    internal class BaseTollCalc
    {
        public double MaxTaxAmntPerDay { get; set; }

        // In minute
        public int ScopeFreeToll { get; set; }

        public Vehicle InputVehicle { get; set; }

        public List<DateTime> InputDates { get; set; }

        public List<TollFeePeriod> TimeRule { get; set; }
        public Dictionary<int, List<int>> TollFreeDaysByMonth { get; set; }

        public DataTable TollAmntDetailed = new DataTable();

        public DataTable TollAmntRevisedByTimeScope = new DataTable();

        public DataTable TollTaxAmntDatTable = new DataTable();

        List<double> _TollAmountInScope = new List<double>();

        public void SetRules(List<TollFeePeriod> timeRule, Dictionary<int, List<int>> tollFreeDaysByMonth)
        {
            TimeRule = timeRule;
            TollFreeDaysByMonth = tollFreeDaysByMonth;
        }

        public virtual void CalcTollAmnt()
        {
            CalcTollAmntDetails();
            calcTollAmntByRules();
        }

        private DataTable CalcTollAmntDetails()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DateTime" ,typeof(DateTime));
            dt.Columns.Add("Date");
            dt.Columns.Add("Time");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Description");
            dt.Columns.Add("ScopeId");

            DateTime scopeHolder = DateTime.Now;
            for (int i = 0; i < InputDates.Count; i++)
            {
                CalculateFigureToll(InputDates[i], dt);
            }
            TollAmntDetailed = dt.Copy();
            return TollAmntDetailed;
        }

        private void CalculateFigureToll(DateTime dateTime, DataTable dt)
        {

            if (!CheckFreeOfChargOptions(dateTime, dt))
            {
                GetTollFee(dateTime, dt);
            }


        }

        private bool CheckFreeOfChargOptions(DateTime dateTime, DataTable dt)
        {
            if (InputVehicle.IsFree)
            {
                //_tollAmount = 0;
                //_Description = "The vehicle is not subject to toll charges.";
                dt.Rows.Add(dateTime ,dateTime.ToString("yyyy/MM/dd"), dateTime.ToString("HH:mm:ss"), 0, "The vehicle is not subject to toll charges." , "");
                return true;
            }
            else if (CheckfreeDay(dateTime))
            {
                //_tollAmount = 0;
                //_Description = "No toll is charged on this day.";
                dt.Rows.Add(dateTime, dateTime.ToString("yyyy/MM/dd"), dateTime.ToString("HH:mm:ss"), 0, "No toll is charged on this day." , "");

                return true;
            }
            return false;
        }
        private bool CheckfreeDay(DateTime date)
        {

            int month = date.Month;
            int day = date.Day;

            if (TollFreeDaysByMonth.TryGetValue(month, out var days))
            {
                if (days == null || days.Contains(day))
                    return true;
            }

            return false;

        }

        public void GetTollFee(DateTime time, DataTable dt)
        {
            TimeSpan currentTime = time.TimeOfDay;

            foreach (var period in TimeRule)
            {

                if (period.StartTime > period.EndTime)
                {
                    if (currentTime >= period.StartTime || currentTime <= period.EndTime)
                        //_tollAmount = period.Fee;
                        dt.Rows.Add(time, time.ToString("yyyy/MM/dd"), time.ToString("HH:mm:ss"), period.Fee, "" , "");
                }
                else
                {
                    if (currentTime >= period.StartTime && currentTime <= period.EndTime)
                        //_tollAmount = period.Fee;
                        dt.Rows.Add(time, time.ToString("yyyy/MM/dd"), time.ToString("HH:mm:ss"), period.Fee, "" , "");
                }
            }



        }
       
        /// <summary>
        /// In this methode we must check the total amount of toll in a day and check the time Scope
        /// </summary>
        private void calcTollAmntByRules()
        {
            DataTable AllTollDataWithValu = TollAmntDetailed.AsEnumerable().Where(r => r["Amount"].ToString() != "0").CopyToDataTable();

            MakeScopeDataTableID(AllTollDataWithValu);

            ReviseTollValueByTimeSpan(AllTollDataWithValu);

            ReviseTollValueByDayMaxAmnt();
        }

        private void MakeScopeDataTableID(DataTable allTollDataWithValu)
        {
            int scopeId = 0;
            for (int i = 0; i < allTollDataWithValu.Rows.Count; i++)
            {
                DateTime _fullDate = Convert.ToDateTime(allTollDataWithValu.Rows[i]["DateTime"]);
                DateTime _date = Convert.ToDateTime(allTollDataWithValu.Rows[i]["Date"]);
                string _scopId = allTollDataWithValu.Rows[i]["ScopeId"].ToString();

                if (_scopId == "")
                {
                    allTollDataWithValu.Rows[i]["ScopeId"] = scopeId;
                    allTollDataWithValu.AcceptChanges();
                    SetScopeIdForNextSameRecors(allTollDataWithValu , scopeId , _fullDate);
                    scopeId++;
                }
            }
        }

        private void SetScopeIdForNextSameRecors(DataTable allTollDataWithValu, int scopeId , DateTime mainDate)
        {
            for (int i = 0; i < allTollDataWithValu.Rows.Count; i++)
            {
                DateTime _fullDate = Convert.ToDateTime(allTollDataWithValu.Rows[i]["DateTime"]);
                DateTime _date = Convert.ToDateTime(allTollDataWithValu.Rows[i]["Date"]);
                string _scopId = allTollDataWithValu.Rows[i]["ScopeId"].ToString();

                if (_scopId == "")
                {
                    if (mainDate.AddMinutes(ScopeFreeToll) >= _fullDate)
                    {
                        allTollDataWithValu.Rows[i]["ScopeId"] = scopeId;
                    }
                }
            }

            allTollDataWithValu.AcceptChanges();
        }

        private void ReviseTollValueByTimeSpan(DataTable allTollDataWithValu)
        {
            for (int i = 0; i < allTollDataWithValu.Rows.Count; i++)
            {

                DateTime _fullDate = Convert.ToDateTime(allTollDataWithValu.Rows[i]["DateTime"]);

                DateTime _date = Convert.ToDateTime(allTollDataWithValu.Rows[i]["Date"] + " " + allTollDataWithValu.Rows[i]["Time"]);

                double _amnt = Convert.ToDouble(allTollDataWithValu.Rows[i]["Amount"]);

                string _desc = allTollDataWithValu.Rows[i]["Description"].ToString();

                string _scopeId = allTollDataWithValu.Rows[i]["ScopeId"].ToString();

                double maxValueInThisScope = allTollDataWithValu.AsEnumerable().Where(r => r["ScopeId"].ToString() == _scopeId).Max(r => Convert.ToDouble(r["Amount"]));

                if (_amnt < maxValueInThisScope)
                {
                    allTollDataWithValu.Rows[i]["Amount"] = "0";
                    allTollDataWithValu.Rows[i]["Description"] = "It was removed due to higher amount of charges during the specified time period.";
                    allTollDataWithValu.AcceptChanges();
                }

            }

            TollAmntRevisedByTimeScope = allTollDataWithValu.Copy();
        }

        private void ReviseTollValueByDayMaxAmnt()
        {
            DataTable dt_SumTollAmnt = TollAmntRevisedByTimeScope.AsEnumerable()
                .GroupBy(r => r["Date"])
                .Select(s => { 
                    var row = TollAmntRevisedByTimeScope.NewRow(); 
                    row["Date"] = s.Key; 
                    row["Amount"] = s.Sum(r => Convert.ToDouble(r["Amount"]));
                    return row; 
                }).CopyToDataTable();

            for (int i = 0; i < dt_SumTollAmnt.Rows.Count; i++)
            {
                double _amnt = Convert.ToDouble(dt_SumTollAmnt.Rows[i]["Amount"]);

                if (_amnt > MaxTaxAmntPerDay)
                {
                    dt_SumTollAmnt.Rows[i]["Amount"] = MaxTaxAmntPerDay;
                    dt_SumTollAmnt.AcceptChanges();
                }

            }

            TollTaxAmntDatTable = dt_SumTollAmnt.Copy();
        }

        public double GetTotalTollTaxAmount()
        {
            return TollTaxAmntDatTable.AsEnumerable().Sum(x => Convert.ToDouble(x["Amount"]));
        }
    }


}