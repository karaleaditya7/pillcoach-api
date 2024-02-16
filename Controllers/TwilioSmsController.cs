using OntrackDb.Repositories;
using System.Threading.Tasks;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace OntrackDb.Controllers;

public class TwilioSmsController : TwilioController
{
    readonly IPatientData _patientData;

    public TwilioSmsController(IPatientData patientData)
    {
        _patientData = patientData;
    }

    public async Task<TwiMLResult> WebHook(SmsRequest incomingMessage)
    {
        var messagingResponse = new MessagingResponse();

        if (incomingMessage != null)
        {
            if ("stop".Equals(incomingMessage.Body, System.StringComparison.OrdinalIgnoreCase))
            {
                messagingResponse.Message($"Thank you for your response.");

                try
                {
                    var patient = await _patientData.GetPatientByContactNumber(incomingMessage.From);

                    if (patient != null)
                    {
                        await _patientData.UpdatePatientConsentAsync(patient.Id, "birthday-sms", false);
                    }
                }
                catch { }
            }
        }

        return TwiML(messagingResponse);
    }
}
