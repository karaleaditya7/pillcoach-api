using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OntrackDb.Context;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio.Jwt;
using Twilio.Jwt.AccessToken;
using Twilio.Jwt.Client;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace OntrackDb.Service
{
    public class CompanyService : ICompanyService
    {
        private IConfiguration _configuration;
        private readonly IPatientData _patientData;
        public CompanyService(IConfiguration configuration, IPatientData patientData)
        {
            _configuration = configuration;
            _patientData = patientData;
         
        }
        public string GenerateAuthTokenForSms(string identity)
        {
             string twilioAccountSid = _configuration["twilioAccountSid"];
             string twilioApiKey = _configuration["twilioApiKey"];
             string twilioApiSecret = _configuration["twilioApiSecret"];

  
            string serviceSid = _configuration["serviceSid"];
            
       

            var grant = new ChatGrant
            {
                ServiceSid = serviceSid
            };

            var grants = new HashSet<IGrant>
            {
                { grant }
            };


            var token = new Token(
                twilioAccountSid,
                twilioApiKey,
                twilioApiSecret,
                identity,
                grants: grants);

            return token.ToJwt();
        }

        public string GenerateAuthTokenForVoice(string identity)
        {
            string twilioAccountSid = _configuration["twilioAccountSid"];
            string authToken = _configuration["twimlAuthToken"];
            string serviceSid = _configuration["twimlServiceSid"];

            var scopes = new HashSet<IScope>
            {
                new OutgoingClientScope(serviceSid),
                new IncomingClientScope(identity)
            };

            var capability = new ClientCapability(twilioAccountSid, authToken, scopes: scopes);
            return capability.ToJwt();
        }

        public VoiceResponse VoiceCall(string to, string callerId)
        {
          
            var twiml = new VoiceResponse();

       
            var dial = new Dial(callerId: callerId);
            dial.Number(to);
            twiml.Append(dial);
            
            return twiml;
        
        }

        public string GenerateAuthTokenForVideo(string identity)
        {
            var accountSid = _configuration["twilioAccountSid"];
            var apiKey = _configuration["twilioApiKey"];
            var apiSecret = _configuration["twilioApiSecret"];

       
            var grant = new VideoGrant();
   
            var grants = new HashSet<IGrant> { grant };

         
            var token = new Token(accountSid, apiKey, apiSecret, identity: identity, grants: grants);

            return token.ToJwt();
            
        }

        public  string GenerateAuthTokenForPatientVideoCall(int identity)
        {
            var accountSid = _configuration["twilioAccountSid"];
            var apiKey = _configuration["twilioApiKey"];
            var apiSecret = _configuration["twilioApiSecret"];

            var patient = _patientData.GetPatientById(identity);

            if (patient != null)
            {
               
                var grant = new VideoGrant();
           
                var grants = new HashSet<IGrant> { grant };

      
                var token = new Token(accountSid, apiKey, apiSecret, identity: identity.ToString(), grants: grants);

                return token.ToJwt();
            }

            else
            {
                return "Patient is not available";
            }
          

        }
    }
}
