using System;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IRedirectionSecurityService
    {
        bool IsValidHost(Uri host);
    }
}