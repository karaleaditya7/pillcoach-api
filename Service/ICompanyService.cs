using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Twilio.TwiML;

namespace OntrackDb.Service
{
    public interface ICompanyService
    {
        string GenerateAuthTokenForSms(string identity);
        string GenerateAuthTokenForVoice(string identity);

        string GenerateAuthTokenForVideo(string identity);
        VoiceResponse VoiceCall(string to, string callerId);

        string GenerateAuthTokenForPatientVideoCall(int identity);
    }
}
