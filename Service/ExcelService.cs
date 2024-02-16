using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System.Data;

namespace OntrackDb.Service
{
    public class ExcelService:IExcelService   
    {
        
        private readonly ApplicationDbContext _applicationDbcontext;
 

        public ExcelService( ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        
        public void InsertIntoDBForExcelDataReader( DataSet dsexcelRecords)
        {
            for (int t = 0; t < dsexcelRecords.Tables.Count; t++)
            {
                DataTable dtMedicationRecords = dsexcelRecords.Tables[t];
                if (dsexcelRecords.Tables[t].TableName == "RASA" || dsexcelRecords.Tables[t].TableName == "Diabetes" ||
                                    dsexcelRecords.Tables[t].TableName == "Sacubitril_Valsartan" || dsexcelRecords.Tables[t].TableName == "Statins" || dsexcelRecords.Tables[t].TableName == "Insulins")
                {
                    int count = 0; 

                    for (int i = 1; i < dtMedicationRecords.Rows.Count; i++)
                    {
    
                        _applicationDbcontext.Pdc_Medications.Add(new Pdc_Medication
                        {
                            value_set_id = dtMedicationRecords.Rows[i][0].ToString(),
                            value_set_subgroup = dtMedicationRecords.Rows[i][1].ToString(),
                            value_set_item = dtMedicationRecords.Rows[i][2].ToString(),
                            code_type = dtMedicationRecords.Rows[i][3].ToString(),
                            code = dtMedicationRecords.Rows[i][4].ToString(),
                            description = dtMedicationRecords.Rows[i][5].ToString(),
                            route = dtMedicationRecords.Rows[i][6].ToString(),
                            dosage_form = dtMedicationRecords.Rows[i][7].ToString(),
                            ingredient = dtMedicationRecords.Rows[i][8].ToString(),
                            strength = dtMedicationRecords.Rows[i][9].ToString(),
                            units = dtMedicationRecords.Rows[i][10].ToString(),
                            from_date = dtMedicationRecords.Rows[i][11].ToString(),
                            thru_date = dtMedicationRecords.Rows[i][12].ToString(),
                            attribute_type = dtMedicationRecords.Rows[i][13].ToString(),
                            attribute_value = dtMedicationRecords.Rows[i][14].ToString(),
                            category = dsexcelRecords.Tables[t].TableName
                           

                        }); 
                        count++;
                       
                        if (count == 500)
                        {
                             _applicationDbcontext.SaveChanges();
                            count = 0;
                        }
                       
                    }
                     _applicationDbcontext.SaveChanges();
                  
                }
            }
        }
    }
}
