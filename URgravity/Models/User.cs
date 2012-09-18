using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace URgravity.Models
{
    public class User
    {
        public int userId { get; set; }

        //FitbitData
        //User profile
        public Boolean HasFitBitData { get; set; }
        public int userIdWithings { get; set; }
        public string FitBitEncodedID { get; set; }
        public string Avatar { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DistanceUnit { get; set; }
        public string Gender { get; set; }

        public string WeightFitBit { get; set; }
        public string Height { get; set; }





        //FITBIT data

        public int ActualCalories { get; set; }
        public virtual ICollection<Calories> CaloriesList { get; set; }
        public string ImageCaloriesUrl { get; set; }


        public int ActualMinutesAsSleep { get; set; }
        public virtual ICollection<MinutesAsSleep> MinutesAsSleepList { get; set; }
        public string ImageMinutesAsSleepUrl { get; set; }


        public int ActualTotalNumberOfSteps { get; set; }
        public virtual ICollection<TotalNumberOfSteps> TotalNumberOfStepsList { get; set; }
        public string ImageTotalNumberOfStepsUrl { get; set; }

        public int ActualTotalFloorsClimbled { get; set; }
        public virtual ICollection<TotalFloorsClimbled> TotalFloorsClimbledList { get; set; }
        public string ImageTotalFloorsClimbledUrl { get; set; }




        //Withings Data

        public Boolean HasWithings { get; set; }

        public double ActualWeight { get; set; }
        public virtual ICollection<Weight> WeightList { get; set; }
        public string ImageWeightUrl { get; set; }

        public double ActualFatFreeMass { get; set; }
        public virtual ICollection<FatFreeMass> FatFreeMassList { get; set; }
        public string ImageFatFreeMassUrl { get; set; }

        public double ActualFatRation { get; set; }
        public virtual ICollection<FatRation> FatRationList { get; set; }
        public string ImageFatRationUrl { get; set; }

        public double ActualFatMassWeight { get; set; }
        public virtual ICollection<FatMassWeight> FatMassWeightList { get; set; }
        public string ImageFatMassWeightUrl { get; set; }



        public Boolean HasOpenTrack { get; set; }


    }






    public class Calories
    {
        public int CaloriesId { get; set; }
        public int value { get; set; }
        public string date { get; set; }

    }


    public class MinutesAsSleep
    {
        public int MinutesAsSleepId { get; set; }
        public int value { get; set; }
        public string date { get; set; }

    }

    public class TotalNumberOfSteps
    {
        public int TotalNumberOfStepsId { get; set; }
        public int value { get; set; }
        public string date { get; set; }

    }

    public class TotalFloorsClimbled
    {
        public int TotalFloorsClimbledId { get; set; }
        public int value { get; set; }
        public string date { get; set; }

    }




    /// <summary>
    /// Classes Withing
    /// </summary>
    public class Weight
    {
        public int weightId { get; set; }
        public double value { get; set; }
        public System.DateTime date { get; set; }

    }

    public class FatFreeMass
    {
        public int FatFreeMassId { get; set; }
        public double value { get; set; }
        public System.DateTime date { get; set; }

    }

    public class FatRation
    {
        public int FatRationId { get; set; }
        public double value { get; set; }
        public System.DateTime date { get; set; }

    }
    public class FatMassWeight
    {
        public int FatMassWeightId { get; set; }
        public double value { get; set; }
        public System.DateTime date { get; set; }

    }

}