using PaymentServices.Types;

namespace LearnSolidAndTesting.Data
{
    public interface IDataStore
    {
        Account GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}
