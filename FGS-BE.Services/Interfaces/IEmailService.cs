using FGS_BE.Repo.Abstracts.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(MailRequest mailRequest);
        //register
        Task SendOtpMail(string name, string otpText, string email);
        //forgot password
        Task SendOtpMailFP(string name, string otpText, string email);
        string GenerateRandomNumber();
    }
}
