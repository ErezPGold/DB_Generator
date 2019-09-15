using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DB_Generator
{
    public class MainWindowViewModel
    {
        public ICommand AddToDB { get; set; }
        public ICommand DeleteDB { get; set; }
        
        public WorkerClass MyProgressWorker { get; set; }

        public string AirlineCompaniesNo { get; set; }
        public string CustomersNo { get; set; }
        public string FlightsPerCompany { get; set; }
        public string TicketsPerCustomer { get; set; }
        public string CountriesNoToAdd { get; set; }

        public static ObservableCollection<string> ListForLog;

        public MainWindowViewModel()
        {
            MyProgressWorker = new WorkerClass();
            ListForLog = LoadListBoxData();
            
            AddToDB = new DelegateCommand(
                () =>
                {
                    AddAllDataToDB();
                }
                );

            DeleteDB = new DelegateCommand(
                () =>
                {
                    DeleteTheDB();
                }
                );            
        }

        private ObservableCollection<string> LoadListBoxData()
        {
            ObservableCollection<string> itemsList = new ObservableCollection<string>();
            itemsList.Add("Coffie");
            itemsList.Add("Tea");
            
            return itemsList;
        }

        private void AddAllDataToDB()
        {
            ListForLog.Add("");
            ListForLog.Add("Starting Add All Data To DB");
            ListForLog.Add("");
            MyProgressWorker.GenerateTheDB(AirlineCompaniesNo, CustomersNo, FlightsPerCompany,
                TicketsPerCustomer, CountriesNoToAdd);            
        }

        private void DeleteTheDB()
        {
            ListForLog.Add("Delete The DB");
            MyProgressWorker.DeleteTheDB();
        }        
    }
}
