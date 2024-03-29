using System;

namespace LegacyApp;

public interface ICreditLimitService
{
    int GetClientCreditLimit(string LastName, DateTime birthday);
}