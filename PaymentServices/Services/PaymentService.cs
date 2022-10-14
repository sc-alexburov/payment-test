using System;
using System.Configuration;
using LearnSolidAndTesting.Data;
using PaymentServices.Data;
using PaymentServices.Types;

namespace PaymentServices.Services
{
    public class PaymentService : IPaymentService
    {
        private IDataStore _accountDataStore;
        public PaymentService(IDataStore accountDataStore)
        {
            _accountDataStore = accountDataStore;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult();

            result.Success = ValidateMakePaymentRequest(request);

            if(result.Success == false)
            {
                return result;
            }

            var account = _accountDataStore.GetAccount(request.DebtorAccountNumber);


            if (account == null)
            {
                result.Success = false;
            }
            else
            {
                result.Success = ValidatePaymentState(account, request);

                if (result.Success)
                {
                    account.Balance -= request.Amount;

                    _accountDataStore.UpdateAccount(account);
                }
            }

            return result;
        }
        private bool ValidatePaymentState(Account account, MakePaymentRequest request)
        {
            bool isPaymentValid = true;

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        isPaymentValid = false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        isPaymentValid = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        isPaymentValid = false;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        isPaymentValid = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        isPaymentValid = false;
                    }
                    break;
            }

            return isPaymentValid;
        }

        private bool ValidateMakePaymentRequest(MakePaymentRequest request)
        {
            if  (request == null 
                || string.IsNullOrEmpty(request.DebtorAccountNumber) 
                || string.IsNullOrEmpty(request.CreditorAccountNumber)
                || request.Amount <= 0) { return false; } return true;
        }
    }
}
