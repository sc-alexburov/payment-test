using System;
using System.Configuration;
using PaymentServices.Data;

namespace LearnSolidAndTesting.Data
{
    public class DataStoreType
    {
        private static IDataStore _dataStore;
        protected DataStoreType() 
        {
            // since app settings could be changed once per build I'm sure that
            // data store type will be one for the entire life of the application
            // thats i did that in Singleton style

            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            if (string.IsNullOrEmpty(dataStoreType))
            {
                throw new ArgumentNullException(nameof(dataStoreType));
            }

            if (dataStoreType == "Backup")
            {
                _dataStore = new BackupAccountDataStore();
            }
            else
            {
                _dataStore = new AccountDataStore();
            }
        }
        public static IDataStore GetDataStoreType()
        {
            return _dataStore ?? throw new ArgumentNullException(nameof(_dataStore));   
        }
    }
}
