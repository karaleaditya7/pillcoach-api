using System.Data;

namespace OntrackDb.Service
{
    public interface IExcelService
    {
        void InsertIntoDBForExcelDataReader( DataSet dsexcelRecords);
    }
}
