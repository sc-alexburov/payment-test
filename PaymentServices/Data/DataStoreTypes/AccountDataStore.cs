using PaymentServices.Types;
using LearnSolidAndTesting.Data;

namespace PaymentServices.Data
{
    public class AccountDataStore : IDataStore
    {
        public Account GetAccount(string accountNumber)
        {
            return new Account();
        }

        public void UpdateAccount(Account account)
        {
        }
    }
}
