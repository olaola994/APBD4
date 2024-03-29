using System;
using System.Diagnostics.SymbolStore;

namespace LegacyApp
{
    public class UserService
    {
        private IClientRepository _clientRepository;
        private ICreditLimitService _creditLimitService;

        public UserService()
        {
            _clientRepository = new ClientRepository();
            _creditLimitService = new UserCreditService();
        }
        public UserService(IClientRepository clientRepository, ICreditLimitService creditLimitService)
        {
            _clientRepository = clientRepository;
            _creditLimitService = creditLimitService;
        }
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            var isValidData = ValidateData(firstName, lastName);
            if (!isValidData)
            {
                return false;
            }

            var isValidDEmail = ValidateEmail(email);
            if (!isValidDEmail)
            {
                return false;
            }
            
            var isValidAge = ValidateAge(dateOfBirth);
            if (!isValidAge)
            {
                return false;
            }

            var client = _clientRepository.GetClientById(clientId);

            var user = CreateUser(client, dateOfBirth, email, firstName, lastName);

            SetClientCreditLimits(user, client);

            bool isCreditLimitBelowThreshol = ValidateCreditLimitBelowThreshold(user);
            if (!isCreditLimitBelowThreshol)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        public Boolean ValidateData(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ValidateEmail(string email)
        {
            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            return true;
        }

        public Boolean ValidateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public User CreateUser(Object client, DateTime birthdate, string email, string firstName, string lastName)
        {
            return new User()
            {
                Client = client,
                DateOfBirth = birthdate,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public void SetClientCreditLimits(User user, Client client)
        {
            
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else if (client.Type == "ImportantClient")
            {
                int creditLimit = _creditLimitService.GetClientCreditLimit(user.LastName, user.DateOfBirth);
                creditLimit = creditLimit * 2;
                user.CreditLimit = creditLimit;
            }
            else
            {
                user.HasCreditLimit = true;
                int creditLimit = _creditLimitService.GetClientCreditLimit(user.LastName, user.DateOfBirth);
                user.CreditLimit = creditLimit;
            }
        }
        public bool ValidateCreditLimitBelowThreshold(User user)
        {
            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            return true;
        }
    }
    
}
