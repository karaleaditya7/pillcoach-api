using Newtonsoft.Json.Linq;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IOntrackParser
    {
        Patient ParsePatient(JObject obj);
        Pharmacy ParsePharmacy(JObject obj);
        Medication ParseMedication(JObject obj);
        Doctor ParseDoctor(JObject obj);

    }
}
