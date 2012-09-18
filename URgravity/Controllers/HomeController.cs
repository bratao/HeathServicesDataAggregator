using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using URgravity.Filters;
using URgravity.Models;
using WebMatrix.WebData;


namespace URgravity.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {


        private UserContext db = new UserContext();


        public ActionResult AllData()
        {
            User user = new User();

            Boolean foundRecord = false;

            foreach (User dbuser in db.Users)
            {
                if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    foundRecord = true;
                    user = dbuser;
                }

            }


            return View(user);
        }
        

        public ActionResult Index()
        {


            User user = new User();

            Boolean foundRecord = false;

            foreach (User dbuser in db.Users)
            {
                if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    foundRecord = true;
                    user = dbuser;
                }

            }

            bool first = true;
            if (user.HasFitBitData)
            {

            //Get the data to draw the main graph

            //Calories
           string caloriesStr = "";

          

           foreach (Calories cal in user.CaloriesList)
           {
               if (!first)
               {

                   caloriesStr += ",";
               }
               caloriesStr += cal.value.ToString();
               first = false;
           }

           ViewBag.Calories = caloriesStr;

            }
            //Weight
            if (user.HasWithings)
            {
                string weightStr = "";

                first = true;



                int start = user.WeightList.Count - 31;

                if (start < 0)
                    start = 0;
                for (int i = start; i < user.WeightList.Count; i++)
                {
                    Weight wei = user.WeightList.ElementAt(i);
                    if (!first)
                    {

                        weightStr += ",";
                    }
                    weightStr += ((wei.value)).ToString();
                    first = false;
                }


                ViewBag.Weight = weightStr;
            }

           if (user.HasFitBitData)
           {

               //Calories Dates
               string caloriesDateStr = "";

               first = true;

               foreach (Calories cal in user.CaloriesList)
               {
                   if (!first)
                   {

                       caloriesDateStr += ",";
                   }
                   caloriesDateStr += "'" + cal.date.ToString() + "'";
                   first = false;
               }

               ViewBag.CaloriesDates = caloriesDateStr;

           }

            return View(user);
        
        }

        public ActionResult Home()
        {

            ViewBag.Message = "Hi, this is our Home page.";


            return View();
        }

        public ActionResult Challenges()
        {

            ViewBag.Message = "Hi, this is our Challenges page.";


            return View();
        }

        public ActionResult DashBoard()
        {
            User user = new User();

            Boolean foundRecord = false;

            foreach (User dbuser in db.Users)
            {
                if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    foundRecord = true;
                    user = dbuser;
                }

            }


            return View(user);
        }

        private User GetFitBitData(User user)
        {
            FitBitClient client = new FitBitClient(Request.Url, HttpContext.Session.SessionID);
            string content = client.GetProfile();
            if (client.isAuthorizated())
            {
                var feedDocument = XDocument.Parse(content);
                var userXml = feedDocument.Element("result").Element("user");
                var fitBitId = userXml.Element("encodedId").Value;


                    //Get data from FitBit
                    user.userId = WebSecurity.GetUserId(User.Identity.Name);
                    user.City = userXml.Element("city").Value;
                    user.FitBitEncodedID = userXml.Element("encodedId").Value;
                    user.DisplayName = userXml.Element("displayName").Value;
                    user.FullName = userXml.Element("fullName").Value;
                    user.City = userXml.Element("city").Value;
                    user.State = "WA";
                    user.DistanceUnit = userXml.Element("distanceUnit").Value;
                    user.Gender = userXml.Element("gender").Value;
                    user.Avatar = userXml.Element("avatar").Value;

                    
                    
                    user.Height = userXml.Element("height").Value;
                    user.WeightFitBit = userXml.Element("weight").Value;



                    //GET Calories
                    var data = client.GetCalories();

                    var collection = new List<Calories>();

                    user.CaloriesList = collection;

                    foreach (KeyValuePair<string, int> kvp in data)
                    {
                        Calories Calories = new Calories();
                        Calories.date = kvp.Key;
                        Calories.value = kvp.Value;

                        user.CaloriesList.Add(Calories);
                    }
                    user.ActualCalories = data.Values[data.Count - 1];

                    user = GetCaloriesImage(user);


                    //GET MinutesAsSleep
                    var dataMinutesAsSleep = client.GetMinutesAsSleep();

                    var collectionMinutesAsSleep = new List<MinutesAsSleep>();

                    user.MinutesAsSleepList = collectionMinutesAsSleep;

                    foreach (KeyValuePair<string, int> kvp in dataMinutesAsSleep)
                    {
                        MinutesAsSleep MinutesAsSleep = new MinutesAsSleep();
                        MinutesAsSleep.date = kvp.Key;
                        MinutesAsSleep.value = kvp.Value;

                        user.MinutesAsSleepList.Add(MinutesAsSleep);
                    }
                    user.ActualMinutesAsSleep = dataMinutesAsSleep.Values[dataMinutesAsSleep.Count - 1];

                    user = GetMinutesAsSleepImage(user);

                    //GET TotalNumberOfSteps
                    var dataTotalNumberOfSteps = client.GetTotalNumberOfSteps();

                    var collectionTotalNumberOfSteps = new List<TotalNumberOfSteps>();

                    user.TotalNumberOfStepsList = collectionTotalNumberOfSteps;

                    foreach (KeyValuePair<string, int> kvp in dataTotalNumberOfSteps)
                    {
                        TotalNumberOfSteps TotalNumberOfSteps = new TotalNumberOfSteps();
                        TotalNumberOfSteps.date = kvp.Key;
                        TotalNumberOfSteps.value = kvp.Value;

                        user.TotalNumberOfStepsList.Add(TotalNumberOfSteps);
                    }
                    user.ActualTotalNumberOfSteps = dataTotalNumberOfSteps.Values[dataTotalNumberOfSteps.Count - 1];

                    user = GetTotalNumberOfStepsImage(user);

                    //GET TotalFloorsClimbled
                    var dataTotalFloorsClimbled = client.GetTotalFloorsClimbled();

                    var collectionTotalFloorsClimbled = new List<TotalFloorsClimbled>();

                    user.TotalFloorsClimbledList = collectionTotalFloorsClimbled;

                    foreach (KeyValuePair<string, int> kvp in dataTotalFloorsClimbled)
                    {
                        TotalFloorsClimbled TotalFloorsClimbled = new TotalFloorsClimbled();
                        TotalFloorsClimbled.date = kvp.Key;
                        TotalFloorsClimbled.value = kvp.Value;

                        user.TotalFloorsClimbledList.Add(TotalFloorsClimbled);
                    }
                    user.ActualTotalFloorsClimbled = dataTotalFloorsClimbled.Values[dataTotalFloorsClimbled.Count - 1];

                    user = GetTotalFloorsClimbledImage(user);




                    user.HasFitBitData = true;
                    db.SaveChanges();
                    return user;
                

            }
            else
            {

                Response.Redirect(content);
                return user;
            }

        }




        public User GetCaloriesImage(User user)
        {

            List<int> CaloriesList = new List<int>();
            List<string> CaloriesDatesList = new List<string>();

            foreach (Calories kvp in user.CaloriesList)
            {


                CaloriesDatesList.Add(kvp.date);
                CaloriesList.Add(kvp.value);
            }


            Chart CaloriesChart = new Chart(width: 800, height: 400);
            CaloriesChart.AddSeries(chartType: "Line", xValue: CaloriesDatesList,
                yValues: CaloriesList);
            //FatFreeMassChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userCalories"), user.userId.ToString() + ".jpeg");
            CaloriesChart.Save(path);

            user.ImageFatFreeMassUrl = "../Images/userCalories/" + user.userId + ".jpeg";

            return user;

        }




        public User GetMinutesAsSleepImage(User user)
        {

            List<int> MinutesAsSleepList = new List<int>();
            List<string> MinutesAsSleepDatesList = new List<string>();

            foreach (MinutesAsSleep kvp in user.MinutesAsSleepList)
            {


                MinutesAsSleepDatesList.Add(kvp.date);
                MinutesAsSleepList.Add(kvp.value);
            }


            Chart MinutesAsSleepChart = new Chart(width: 800, height: 400);
            MinutesAsSleepChart.AddSeries(chartType: "Line", xValue: MinutesAsSleepDatesList,
                yValues: MinutesAsSleepList);
            //FatFreeMassChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userMinutesAsSleep"), user.userId.ToString() + ".jpeg");
            MinutesAsSleepChart.Save(path);

            user.ImageMinutesAsSleepUrl = "../Images/userMinutesAsSleep/" + user.userId + ".jpeg";

            return user;

        }

        public User GetTotalNumberOfStepsImage(User user)
        {

            List<int> TotalNumberOfStepsList = new List<int>();
            List<string> TotalNumberOfStepsDatesList = new List<string>();

            foreach (TotalNumberOfSteps kvp in user.TotalNumberOfStepsList)
            {


                TotalNumberOfStepsDatesList.Add(kvp.date);
                TotalNumberOfStepsList.Add(kvp.value);
            }


            Chart TotalNumberOfStepsChart = new Chart(width: 800, height: 400);
            TotalNumberOfStepsChart.AddSeries(chartType: "Line", xValue: TotalNumberOfStepsDatesList,
                yValues: TotalNumberOfStepsList);
            //FatFreeMassChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userTotalNumberOfSteps"), user.userId.ToString() + ".jpeg");
            TotalNumberOfStepsChart.Save(path);

            user.ImageTotalFloorsClimbledUrl = "../Images/userTotalNumberOfSteps/" + user.userId + ".jpeg";

            return user;

        }

        public User GetTotalFloorsClimbledImage(User user)
        {

            List<int> TotalFloorsClimbledList = new List<int>();
            List<string> TotalFloorsClimbledDatesList = new List<string>();

            foreach (TotalFloorsClimbled kvp in user.TotalFloorsClimbledList)
            {


                TotalFloorsClimbledDatesList.Add(kvp.date);
                TotalFloorsClimbledList.Add(kvp.value);
            }


            Chart CaloriesChart = new Chart(width: 800, height: 400);
            CaloriesChart.AddSeries(chartType: "Line", xValue: TotalFloorsClimbledDatesList,
                yValues: TotalFloorsClimbledList);
            //FatFreeMassChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userTotalFloorsClimbled"), user.userId.ToString() + ".jpeg");
            CaloriesChart.Save(path);

            user.ImageTotalFloorsClimbledUrl = "../Images/userTotalFloorsClimbled/" + user.userId + ".jpeg";

            return user;

        }










        public ActionResult FitBit()
        {


                User user = new User();

                Boolean foundRecord = false;

                foreach (User dbuser in db.Users)
                {
                    if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                    {
                        foundRecord = true;
                        user = dbuser;
                    } 

                }


                if (!user.HasFitBitData)
                {
                    user = GetFitBitData(user);

                }

                if (!foundRecord)
                    db.Users.Add(user);

                db.SaveChanges();

                return View(user);
                

        }



        private User GetWithingsData(User user)
        {

            Withings client = new Withings();
            client.Login();



            //GET Weight
            var data = client.GetWeight();

            List<double> weightList = new List<double>();
            List<string> weightDatesList = new List<string>();

            var collection = new List<Weight>();

            user.WeightList = collection;

            foreach (KeyValuePair<System.DateTime, double> kvp in data)
            {
                Weight weight = new Weight();
                weight.date = kvp.Key;
                weight.value = kvp.Value * 2.20462;

                user.WeightList.Add(weight);


                weightDatesList.Add(kvp.Key.ToShortDateString());
                weightList.Add(kvp.Value * 2.20462);
                user.ActualWeight = kvp.Value * 2.20462;
            }
            

            user = GetWeightImage(user);


            //GET FatFreeMass

            var dataFatFreeMass = client.GetFatFreeMass();

            List<double> FatFreeMassList = new List<double>();
            List<string> FatFreeMassDatesList = new List<string>();

            var collectionFatFreeMass = new List<FatFreeMass>();

            user.FatFreeMassList = collectionFatFreeMass;

            foreach (KeyValuePair<System.DateTime, double> kvp in dataFatFreeMass)
            {
                FatFreeMass FatFreeMass = new FatFreeMass();
                FatFreeMass.date = kvp.Key;
                FatFreeMass.value = kvp.Value;

                user.FatFreeMassList.Add(FatFreeMass);


                FatFreeMassDatesList.Add(kvp.Key.ToShortDateString());
                FatFreeMassList.Add(kvp.Value);
            }
            user.ActualFatFreeMass = data.Values[data.Values.Count-1];

            user = GetFatFreeMassImage(user);



            //GET FatRation
            var dataFatRation = client.GetFatRation();

            List<double> FatRationList = new List<double>();
            List<string> FatRationDatesList = new List<string>();

            var collectionFatRation = new List<FatRation>();

            user.FatRationList = collectionFatRation;

            foreach (KeyValuePair<System.DateTime, double> kvp in dataFatRation)
            {
                FatRation FatRation = new FatRation();
                FatRation.date = kvp.Key;
                FatRation.value = kvp.Value;

                user.FatRationList.Add(FatRation);


                FatRationDatesList.Add(kvp.Key.ToShortDateString());
                FatRationList.Add(kvp.Value);
            }
            user.ActualFatRation = data.Values[data.Values.Count - 1];

            user = GetFatRationImage(user);


            //GET FatMassWeight
            var dataFatMassWeight = client.GetFatMassWeight();

            List<double> FatMassWeightList = new List<double>();
            List<string> FatMassWeightDatesList = new List<string>();

            var collectionFatMassWeight = new List<FatMassWeight>();

            user.FatMassWeightList = collectionFatMassWeight;

            foreach (KeyValuePair<System.DateTime, double> kvp in dataFatMassWeight)
            {
                FatMassWeight FatMassWeight = new FatMassWeight();
                FatMassWeight.date = kvp.Key;
                FatMassWeight.value = kvp.Value * 2.20462;

                user.FatMassWeightList.Add(FatMassWeight);


                FatMassWeightDatesList.Add(kvp.Key.ToShortDateString());
                FatMassWeightList.Add(kvp.Value * 2.20462);
            }
            user.ActualFatMassWeight = data.Values[data.Values.Count - 1] * 2.20462;

            user = GetFatMassWeightImage(user);





            user.HasWithings = true;








            return user;




        }

        public User GetWeightImage(User user)
        {

            List<double> weightList = new List<double>();
            List<string> weightDatesList = new List<string>();

            foreach (Weight kvp in user.WeightList)
            {
             

                weightDatesList.Add(kvp.date.ToShortDateString());
                weightList.Add(kvp.value);
            }


            Chart weightChart = new Chart(width: 800, height: 400);
            weightChart.AddSeries(chartType: "Line", xValue: weightDatesList,
                yValues: weightList);
            weightChart.SetYAxis(min: 190, max:230);


            var path = Path.Combine(Server.MapPath("~/Images/userWeight"), user.userId.ToString() + ".jpeg");
            weightChart.Save(path);

            user.ImageWeightUrl = "../Images/userWeight/" + user.userId + ".jpeg";

            return user;


        }

        public User GetFatFreeMassImage(User user)
        {

            List<double> FatFreeMassList = new List<double>();
            List<string> FatFreeMassDatesList = new List<string>();

            foreach (FatFreeMass kvp in user.FatFreeMassList)
            {


                FatFreeMassDatesList.Add(kvp.date.ToShortDateString());
                FatFreeMassList.Add(kvp.value);
            }


            Chart FatFreeMassChart = new Chart(width: 800, height: 400);
            FatFreeMassChart.AddSeries(chartType: "Line", xValue: FatFreeMassDatesList,
                yValues: FatFreeMassList);
            //FatFreeMassChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userFatFreeMass"), user.userId.ToString() + ".jpeg");
            FatFreeMassChart.Save(path);

            user.ImageFatFreeMassUrl = "../Images/userFatFreeMass/" + user.userId + ".jpeg";

            return user;


        }

        public User GetFatRationImage(User user)
        {

            List<double> FatRationList = new List<double>();
            List<string> FatRationDatesList = new List<string>();

            foreach (FatRation kvp in user.FatRationList)
            {


                FatRationDatesList.Add(kvp.date.ToShortDateString());
                FatRationList.Add(kvp.value);
            }


            Chart FatRationChart = new Chart(width: 800, height: 400);
            FatRationChart.AddSeries(chartType: "Line", xValue: FatRationDatesList,
                yValues: FatRationList);
            //FatRationChart.SetYAxis(min: 190, max: 230);



            var path = Path.Combine(Server.MapPath("~/Images/userFatRation"), user.userId.ToString() + ".jpeg");
            FatRationChart.Save(path);

            user.ImageFatRationUrl = "../Images/userFatRation/" + user.userId + ".jpeg";

            return user;


        }

        public User GetFatMassWeightImage(User user)
        {

            List<double> FatMassWeightList = new List<double>();
            List<string> FatMassWeightDatesList = new List<string>();

            foreach (FatMassWeight kvp in user.FatMassWeightList)
            {


                FatMassWeightDatesList.Add(kvp.date.ToShortDateString());
                FatMassWeightList.Add(kvp.value);
            }


            Chart FatMassWeightChart = new Chart(width: 800, height: 400);
            FatMassWeightChart.AddSeries(chartType: "Line", xValue: FatMassWeightDatesList,
                yValues: FatMassWeightList);
            //FatMassWeightChart.SetYAxis(min: 190, max: 230);


            var path = Path.Combine(Server.MapPath("~/Images/userFatMassWeight"), user.userId.ToString() + ".jpeg");
            FatMassWeightChart.Save(path);

            user.ImageFatMassWeightUrl = "../Images/userFatMassWeight/" + user.userId + ".jpeg";

            return user;


        }


        public ActionResult Withings()
        {


            User user = new User();

            Boolean foundRecord = false;

            foreach (User dbuser in db.Users)
            {
                if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    foundRecord = true;
                    user = dbuser;
                }

            }

            if (!foundRecord || (foundRecord && !user.HasWithings))
            {
                user = GetWithingsData(user);
                user.userId = WebSecurity.GetUserId(User.Identity.Name);
            }


            if (!foundRecord)
                db.Users.Add(user);
            db.SaveChanges();


            return View();
        }


        public ActionResult Devices()
        {
            User user = new User();

            Boolean foundRecord = false;

            foreach (User dbuser in db.Users)
            {
                if (dbuser.userId == WebSecurity.GetUserId(User.Identity.Name))
                {
                    foundRecord = true;
                    user = dbuser;
                }

            }

            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
