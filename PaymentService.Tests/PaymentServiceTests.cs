using System;
using NSubstitute;
using NUnit.Framework;
using PaymentServices.Types;
using LearnSolidAndTesting.Data;

namespace PaymentService.Tests
{
    public class PaymentServiceTests
    {

        private IDataStore _dataStore;
        private PaymentServices.Services.PaymentService _ps;


        [SetUp]
        public void Setup()
        {
            _dataStore = Substitute.For<IDataStore>();
            _ps = new PaymentServices.Services.PaymentService(_dataStore);
        }

        [Test]
        public void should_return_succeed_false_when_account_is_null()
        {
            _dataStore.GetAccount(Arg.Any<string>()).Returns((Account)null);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 12.30m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.FasterPayments
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_false_when_allowed_payment_scheme_flag_is_not_faster_payments()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 12.30m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.FasterPayments
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_false_when_allowed_payment_scheme_flag_is_not_basc()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 12.30m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.Bacs
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_false_when_allowed_payment_scheme_flag_is_not_chaps()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 12.30m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.Chaps
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_when_the_balance_is_less_then_the_amount()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 20m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 100m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.FasterPayments
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_when_allowed_payments_is_chaps_and_status_is_not_live()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 20m,
                Status = AccountStatus.Disabled
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 100m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.Chaps
            });

            Assert.That(result.Success == false);
        }

        [Test]
        public void should_return_succeed_when_allowed_payments_is_chaps_and_status_is_live()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 20m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.Chaps
            });

            Assert.That(result.Success == true);
            Assert.That(acc.Balance == (100 - 20));
        }

        [Test]
        public void should_return_succeed_when_the_transaction_is_valid()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 20m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.FasterPayments
            });

            Assert.That(result.Success == true);
            Assert.That(acc.Balance == (100 - 20));
        }
       
        [Test] 
        public void should_return_succeed_when_the_payment_schemes_are_basc()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 20m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = "dasda",
                CreditorAccountNumber = "dasda",
                PaymentScheme = PaymentScheme.Bacs
            });

            Assert.That(result.Success == true);
            Assert.That(acc.Balance == (100 - 20));
        }

        [Test]
        public void should_return_succeed_false_when_payment_request_model_has_invalid_data()
        {
            var acc = new Account()
            {
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = 100m,
                Status = AccountStatus.Live
            };

            _dataStore.GetAccount(Arg.Any<string>()).Returns(acc);

            var result = _ps.MakePayment(new MakePaymentRequest()
            {
                Amount = 20m,
                PaymentDate = new DateTime(),
                DebtorAccountNumber = string.Empty,
                CreditorAccountNumber = string.Empty,
                PaymentScheme = PaymentScheme.Bacs
            });

            Assert.That(result.Success == false);
        }
        
    }

}
