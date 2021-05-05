using System;
using System.Threading.Tasks;

namespace Products.Service.Common
{
    public interface IActivationEmailService
    {
        Task SendActivationEmail(string emailAddress, Guid userId);
    }
}