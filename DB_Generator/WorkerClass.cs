using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DB_Generator
{
    public class WorkerClass : INotifyPropertyChanged
    {
        private BackgroundWorker _bgWorker;
        public event PropertyChangedEventHandler PropertyChanged;
        public GenerateDataAndPutInDB GenerateDataAndPutInDB;
        public GenerateDataAndPutInDB GenerateDataAndPutInDB2;

        private int _workerState;
        public int WorkerState
        {
            get
            {
                return _workerState;
            }
            set
            {
                _workerState = value;
                OnPropertyChanged("WorkerState");
            }
        }
        //number of Data parameters to create:
        private string _AirlineCompaniesNo;
        private string _CustomersNo;
        private string _FlightsPerCompany;
        private string _TicketsPerCustomer;
        private string _CountriesNoToAdd;
        
        //ctor:
        public WorkerClass()
        {
            _bgWorker = new BackgroundWorker();
        }
        
        public void GenerateTheDB(string AirlineCompaniesNo, string CustomersNo, string FlightsPerCompany,
            string TicketsPerCustomer, string CountriesNoToAdd)
        {
            _AirlineCompaniesNo = AirlineCompaniesNo;
            _CustomersNo = CustomersNo;
            _FlightsPerCompany = FlightsPerCompany;
            _TicketsPerCustomer = TicketsPerCustomer;
            _CountriesNoToAdd = CountriesNoToAdd;


            try
            {
                //send all numbers from user to generate DB:
                GenerateDataAndPutInDB = new GenerateDataAndPutInDB(Convert.ToInt32(_AirlineCompaniesNo),
                    Convert.ToInt32(_CustomersNo), Convert.ToInt32(_FlightsPerCompany),
                    Convert.ToInt32(_TicketsPerCustomer), Convert.ToInt32(_CountriesNoToAdd));

                _bgWorker.DoWork += (s, e) =>
                {
                    for (int i = 0; i < 101; i++)
                    {
                        Thread.Sleep(10);
                        WorkerState = i;
                    }
                };

                _bgWorker.RunWorkerAsync();
                GenerateDataAndPutInDB.GenerateData();
            }
            catch (Exception exx)
            {
                //check all strings are numbers:
                if (!Int32.TryParse(_AirlineCompaniesNo, out int i1))
                {
                    MessageBox.Show("Airline Companies No is not A number");
                    MainWindowViewModel.ListForLog.Add("Airline Companies No is not A number");
                }
                if (!Int32.TryParse(_CustomersNo, out int i2))
                {
                    MessageBox.Show("Customers No is not A number");
                    MainWindowViewModel.ListForLog.Add("Customers No is not A number");
                }
                if (!Int32.TryParse(_FlightsPerCompany, out int i3))
                {
                    MessageBox.Show("Flights Per Company is not A number");
                    MainWindowViewModel.ListForLog.Add("Flights Per Company is not A number");
                }
                if (!Int32.TryParse(_TicketsPerCustomer, out int i4))
                {
                    MessageBox.Show("Tickets Per Customer is not A number");
                    MainWindowViewModel.ListForLog.Add("Tickets Per Customer is not A number");
                }
                if (!Int32.TryParse(_CountriesNoToAdd, out int i5))
                {
                    MessageBox.Show("Countries No To Add is not A number");
                    MainWindowViewModel.ListForLog.Add("Countries No To Add is not A number");
                }

                MessageBox.Show("The exception is: " + exx.ToString());
                MainWindowViewModel.ListForLog.Add("The exception of not a number is: " + exx.ToString());
            }           
            
        }

        public void DeleteTheDB()
        {
            GenerateDataAndPutInDB2 = new GenerateDataAndPutInDB();
            GenerateDataAndPutInDB2.DeleteFlightsDB();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
