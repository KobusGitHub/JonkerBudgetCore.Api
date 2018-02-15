using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommsApi.Client.Core;
using CommsApi.Client.Core.Email.Classes;
using CommsApi.Client.Core.Sms.Classes;
using Microsoft.Extensions.Configuration;
using JonkerBudgetCore.Api.Domain.Configuration;

namespace JonkerBudgetCore.Api.Domain.Shared_Services
{
    public class CommsService : ICommsService
    {
        private readonly IConfigurationRoot ConfigurationRoot;
        private readonly CommsApiClientConfiguration config;

        public CommsService(IConfigurationRoot ConfigurationRoot)
        {
            this.config = new CommsApiClientConfiguration();
            this.ConfigurationRoot = ConfigurationRoot;
            ConfigurationRoot.GetSection("CommsApiClientConfiguration").Bind(config);
        }

        public SendEmailBasic GetBaseEmail()
        {
            return new SendEmailBasic()
            {
                Bcc = config.BccRecipients,
                CustomerReference = config.CustomerReference,
                From = config.FromAddress
            };
        }

        public async Task<SendEmailResult> SendEmail(SendEmailBasic email)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                return await comms.EmailApi.SendEmailBasicAsync(email);
            }
        }

        public async Task<ICollection<SendEmailResult>> SendEmails(ICollection<SendEmailBasic> emails)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                var result = new List<SendEmailResult>();

                foreach (var email in emails)
                {
                    result.Add(await comms.EmailApi.SendEmailBasicAsync(email));
                }

                return result;
            }
        }

        public async Task<SendEmailResult> SendEmailStream(SendEmailStream email)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                return await comms.EmailApi.SendEmailAsStreamAsync(email);
            }
        }

        public async Task<ICollection<SendEmailResult>> SendEmailStreams(ICollection<SendEmailStream> emails)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                var result = new List<SendEmailResult>();

                foreach (var email in emails)
                {
                    result.Add(await comms.EmailApi.SendEmailAsStreamAsync(email));
                }

                return result;
            }
        }

        public async Task<SendSmsResult> SendSms(SmsMessage sms)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                return await comms.SmsApi.SendSmsAsync(sms);
            }
        }

        public async Task<ICollection<SendSmsResult>> SendSmses(ICollection<SmsMessage> smses)
        {
            using (CommsApiInterface comms = new CommsApiInterface(config.ApiAddress, config.Username, config.Password))
            {
                var result = new List<SendSmsResult>();

                foreach (var sms in smses)
                {
                    result.Add(await comms.SmsApi.SendSmsAsync(sms));
                }

                return result;
            }
        }
    }
}
