using CommsApi.Client.Core.Email.Classes;
using CommsApi.Client.Core.Sms.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Shared_Services
{
    public interface ICommsService
    {
        SendEmailBasic GetBaseEmail();
        Task<SendEmailResult> SendEmail(SendEmailBasic email);
        Task<ICollection<SendEmailResult>> SendEmails(ICollection<SendEmailBasic> emails);
        Task<SendEmailResult> SendEmailStream(SendEmailStream email);
        Task<ICollection<SendEmailResult>> SendEmailStreams(ICollection<SendEmailStream> emails);
        Task<SendSmsResult> SendSms(SmsMessage sms);
        Task<ICollection<SendSmsResult>> SendSmses(ICollection<SmsMessage> smses);
    }
}
