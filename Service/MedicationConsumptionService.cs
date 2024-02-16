using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class MedicationConsumptionService: IMedicationConsumptionService
    {
        private readonly IMedicationConsumptionData _medicationConsumptionData;
        public MedicationConsumptionService(IMedicationConsumptionData medicationConsumptionData)
        {
            _medicationConsumptionData = medicationConsumptionData;   
        }

       public async Task<Response<MedicationConsumption>> GetRecentMedicationConsumption(string RXNumber, int PatientID)
        {
            Response<MedicationConsumption> response = new Response<MedicationConsumption>();
            var medicationConsumption = await _medicationConsumptionData.GetRecentMedicationConsumption(RXNumber,PatientID);

            if (medicationConsumption == null)
            {
                response.Message = "Error while getting medicationConsumption by id";
                response.Success = false;
                return response;
            }
            response.Success = true;
            response.Message = "medicationConsumption retrived successfully";
            response.Data = medicationConsumption;
            return response;
        }
     
    }
}
