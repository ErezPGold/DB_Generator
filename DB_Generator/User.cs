using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Generator
{
    public class User
    {
        public User(string json)
        {
            //option1: 
            JObject jObject = JObject.Parse(json);
            JToken jResult = jObject["results"];

            JToken allData = jResult[0];
            JProperty a1 = (JProperty)allData.First; // gender            
            Gender = (string)a1.ElementAt(0);
            JProperty name = (JProperty)a1.Next;       // name                      
            Title = (string)name.ElementAt(0)["title"];
            First_Name = (string)name.ElementAt(0)["first"];
            Last_name = (string)name.ElementAt(0)["last"];
            JProperty location = (JProperty)name.Next;
            string street = (string)location.ElementAt(0)["street"];
            string city = (string)location.ElementAt(0)["city"];
            string state = (string)location.ElementAt(0)["state"];
            string postcode = (string)location.ElementAt(0)["postcode"];
            Address = $"Street: {street} City: {city} State: {state} PostCode: {postcode}";
            JProperty email = (JProperty)location.Next;
            JProperty login = (JProperty)email.Next;
            User_Name = (string)login.ElementAt(0)["username"];
            Password = (string)login.ElementAt(0)["password"];
            JProperty dob = (JProperty)login.Next;
            JProperty registered = (JProperty)dob.Next;
            JProperty phonenumber = (JProperty)registered.Next;
            Phone_Number = (string)phonenumber.First;
            JProperty cellnumber = (JProperty)phonenumber.Next;
            Credit_Card_Number = (string)cellnumber.First; //cell instead of creditcard number.
            //email = (string)jUser["email"];
            //players = jResult["players"].ToArray();

            /*
            option2:
            var jsonObject = Json.Decode(json);
            DynamicJsonArray p = (DynamicJsonArray)jsonObject.results;
            DynamicJsonObject a = (DynamicJsonObject)p.FirstOrDefault();
            DynamicJsonObject b = (DynamicJsonObject)p.LastOrDefault();
            var m = a.GetDynamicMemberNames();
            
            players = (DynamicJsonArray) jsonObject.results;
            DynamicJsonObject values = (DynamicJsonObject)players.GetValue(0); //System.Web.Helpers.DynamicJsonObject
            */
            //var a = values.ToString();
            //var b = (string)players.gender;
        }


        public string Gender { get; set; }

        public string Title { get; set; }
        public string First_Name { get; set; }
        public string Last_name { get; set; }
        public string User_Name { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone_Number { get; set; }
        public string Credit_Card_Number { get; set; }
        //public Array players { get; set; }
    }
}
