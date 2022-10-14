using PaymentServices.Data;
using System;
using System.Configuration;

namespace LearnSolidAndTesting.Data
{
    public class DataStoreType
    {
        static IDataStore dataStore;
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
                dataStore = new BackupAccountDataStore();
            }
            else
            {
                dataStore = new AccountDataStore();
            }
        }
        public static IDataStore GetDataStoreType()
        {
            return dataStore ?? throw new ArgumentNullException(nameof(dataStore));   
        }
    }
}
