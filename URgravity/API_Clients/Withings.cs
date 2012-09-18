using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Meiners.Libs.Withings;


namespace URgravity
{
    public class Withings
    {
        Meiners.Libs.Withings.User user;
        GetMeasuresResponse data;
        public void Login()
        {
            Meiners.Libs.Withings.Account userAccount = new Meiners.Libs.Withings.Account("rmiller@urgravity.com", "August2012");

            Meiners.Libs.Withings.GetUserListResponse userList = userAccount.GetUserList();
            user = userList.Users[0];

        }


        public SortedList<System.DateTime, double> GetWeight()
        {

            SortedList<System.DateTime, double> weightList = new SortedList<System.DateTime, double>();
            data = user.ReadMeasures(null,null,null);

            foreach (MeasureGroup measure in data.ReadMeasureGroups())
            {
                Measure[] measures;
                measures = measure.GetMeasures();

                double weight = measures[0].CalculateValue();
                weightList.Add(measure.MeasureGroupDate,weight);

                
            }
            return weightList;
        }

        public SortedList<System.DateTime, double> GetFatFreeMass()
        {

            SortedList<System.DateTime, double> FatFreeMassList = new SortedList<System.DateTime, double>();

            foreach (MeasureGroup measure in data.ReadMeasureGroups())
            {
                Measure[] measures;
                measures = measure.GetMeasures();

                int length = measures.Length;
                if (length < 2)
                    continue;


                double weight = measures[1].CalculateValue();
                FatFreeMassList.Add(measure.MeasureGroupDate, weight);


            }
            return FatFreeMassList;
        }

        public SortedList<System.DateTime, double> GetFatRation()
        {

            SortedList<System.DateTime, double> FatRationList = new SortedList<System.DateTime, double>();

            foreach (MeasureGroup measure in data.ReadMeasureGroups())
            {
                Measure[] measures;
                measures = measure.GetMeasures();

                int length = measures.Length;
                if (length < 3)
                    continue;

                double weight = measures[2].CalculateValue();
                FatRationList.Add(measure.MeasureGroupDate, weight);


            }
            return FatRationList;
        }

        public SortedList<System.DateTime, double> GetFatMassWeight()
        {

            SortedList<System.DateTime, double> FatMassWeightList = new SortedList<System.DateTime, double>();

            foreach (MeasureGroup measure in data.ReadMeasureGroups())
            {
                Measure[] measures;
                measures = measure.GetMeasures();

                int length = measures.Length;
                if (length < 4)
                    continue;

                double weight = measures[3].CalculateValue();
                FatMassWeightList.Add(measure.MeasureGroupDate, weight);


            }
            return FatMassWeightList;
        }

    }
}