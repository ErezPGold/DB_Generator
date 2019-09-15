using FlightsSystem;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DB_Generator
{
    public class GenerateDataAndPutInDB
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ILoginToken IloginAdministrator;
        LoginToken<Administrator> loginTokenAdministrator;
        LoggedInAdministratorFacade administratorFacade;

        ListOfThings listOfThings = new ListOfThings();
        public static SqlCommand cmd = new SqlCommand();

        Random rnd = new Random();
        //number of Data parameters to create:
        private int _AirlineCompaniesNo = 0;
        private int _CustomersNo ;
        private int _FlightsPerCompany ;
        private int _TicketsPerCustomer ;
        private int _CountriesNoToAdd ;

        public GenerateDataAndPutInDB()
        {

        }
        public GenerateDataAndPutInDB(int AirlineCompanies, int CustomersNo, int FlightsPerCompany,
            int TicketsPerCustomer, int CountriesNoToAdd)
        {            
            _AirlineCompaniesNo = AirlineCompanies;// 20;
            _CustomersNo = CustomersNo;// rnd.Next(20, 31);
            _FlightsPerCompany = FlightsPerCompany;
            _TicketsPerCustomer = TicketsPerCustomer;// rnd.Next(3, 7);
            _CountriesNoToAdd = CountriesNoToAdd;
        }

        public void GenerateData()
        {
            log.Info("Creating admin user");
            MainWindowViewModel.ListForLog.Add("Creating admin user");            
            //Creating admin user:
            IloginAdministrator = FlyingCenterSystem.GetFlyingCenterSystemInstance().Login("admin", "9999");
            loginTokenAdministrator = IloginAdministrator as LoginToken<Administrator>;
            administratorFacade = (LoggedInAdministratorFacade)FlyingCenterSystem.GetFlyingCenterSystemInstance().GetFacade(IloginAdministrator);

            //Adds the countries first (foreign key constrain considaration):
            log.Info("Adds the countries first");
            MainWindowViewModel.ListForLog.Add("Adds the countries first");
            for (int i = 0; i < _CountriesNoToAdd; i++)
            {
                administratorFacade.CreateNewCountry(loginTokenAdministrator,
                    new Country() { CountryName = listOfThings.CountriesArray[rnd.Next(listOfThings.CountriesArray.Length)] });
                if (i >= listOfThings.CountriesArray.Length)
                {
                    administratorFacade.CreateNewCountry(loginTokenAdministrator,
                    new Country() { CountryName = RandomString(5) });
                }
            }

            //Get All Countries and add country codes for filling the airline companies:
            log.Info("Adds the Airline Companies");
            MainWindowViewModel.ListForLog.Add("Adds the Airline Companies");
            IList<Country> countries = new List<Country>();
            countries = administratorFacade.GetAllCountries(loginTokenAdministrator);

            //Adds the Airline Companies:  
            for (int i = 0; i < _AirlineCompaniesNo; i++)
            {
                administratorFacade.CreateNewAirline(loginTokenAdministrator, new AirlineCompany()
                {
                    AirLineName = RandomString(5),
                    CountryCode = countries[rnd.Next(0, countries.Count)].CountyID,
                    Password = RandomString(6),
                    UserName = RandomString(4)
                });
            }

            //Adds the Customers:
            log.Info("Adds the Customers");
            MainWindowViewModel.ListForLog.Add("Adds the Customers");
            //FillCustomerTableFromUserApi(CustomersNo); - will not use, site is down (although it's working)
            FillCustomerTableRandomly(rnd, _CustomersNo);

            //Get All AirlineCompanies for the id:
            IList<AirlineCompany> airlineCompanies = administratorFacade.GetAllAirLineCompanies();

            //Adds the Flights Per Company:
            log.Info("Adds the Flights Per Company");
            MainWindowViewModel.ListForLog.Add("Adds the Flights Per Company");
            for (int i = 0; i < _FlightsPerCompany; i++)
            {
                administratorFacade.CreateFlight(loginTokenAdministrator, new Flight()
                {
                    AirLineCompany_ID = airlineCompanies[rnd.Next(0, airlineCompanies.Count)].Airline_ID,
                    Origin_Country_Code = countries[rnd.Next(0, countries.Count)].CountyID,
                    Destination_Country_Code = countries[rnd.Next(0, countries.Count)].CountyID,
                    DepartureTime = createRandomDate(),
                    LandingTime = createRandomDate(),
                    Remaining_Tickets = rnd.Next(5, 51)
                });
            }

            log.Info("Adds the Tickets per Customer");
            MainWindowViewModel.ListForLog.Add("Adds the Tickets per Customer");
            //Get All customers:
            IList<Customer> customers = new List<Customer>();
            customers = administratorFacade.GetAllCustomers(loginTokenAdministrator);

            //Get All flights :
            IList<Flight> flights = new List<Flight>();
            flights = administratorFacade.GetAllFlights();

            //Adds the Tickets per Customer:
            for (int i = 0; i < _TicketsPerCustomer; i++)
            {
                administratorFacade.AddTicketsToCustomer(loginTokenAdministrator,
                    customers[rnd.Next(0, customers.Count)].Customer_ID,
                    flights[rnd.Next(0, flights.Count)].FlightID);
            }

            log.Info("Finished puting data in DB!");
            MainWindowViewModel.ListForLog.Add("Finished puting data in DB!");
        }

        private void FillCustomerTableRandomly(Random rnd, int CustomersNo)
        {
            for (int i = 0; i < CustomersNo; i++)
            {
                Customer customerModel = new Customer()
                {
                    FirstName = RandomString(5),
                    LastName = RandomString(5),
                    Username = RandomString(5),
                    Password = RandomString(5),
                    Address = RandomString(5),
                    PhoneNo = rnd.Next(10000, 100000).ToString(),
                    CreditCardNumber = rnd.Next(1000, 10000).ToString()
                };
                administratorFacade.CreateNewCustomer(loginTokenAdministrator, customerModel);
            }
        }

        private void FillCustomerTableFromUserApi(int CustomersNo)
        {
            WebClient client = new WebClient();
            string json = client.DownloadString("https://randomuser.me/api");
            List<User> users = new List<User>();
            for (int i = 0; i < CustomersNo; i++)
            {
                json = client.DownloadString("https://randomuser.me/api");
                User user = new User(json);
                Customer customerModel = new Customer()
                {
                    FirstName = user.First_Name,
                    LastName = user.Last_name,
                    Username = user.User_Name,
                    Password = user.Password,
                    Address = user.Address,
                    PhoneNo = user.Phone_Number,
                    CreditCardNumber = user.Credit_Card_Number
                };
                administratorFacade.CreateNewCustomer(loginTokenAdministrator, customerModel);
            }
        }

        /// <summary>
        /// a function that creates a random string:
        /// </summary>
        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        /// <summary>
        /// a function that creates a random date:
        /// </summary>
        Random gen = new Random();
        int range = 5 * 365; // 5 years
        public DateTime createRandomDate()
        {
            DateTime randomDate = DateTime.Today.AddDays(-gen.Next(range));
            return randomDate;
        }

        public void DeleteFlightsDB()
        {
            log.Info("Delete Flights DB");
            MainWindowViewModel.ListForLog.Add("Delete Flights DB");

            try
            {
                DeleteDAO.DeleteColumnsOf_FK();
                DeleteDAO.DeleteAllTables();

            }
            catch (Exception exx)
            {
                MessageBox.Show("There is an Exception: " + exx);
            }
            
            log.Info("Delete Success !");
            MainWindowViewModel.ListForLog.Add("Delete Success !");
        }
    }
}
