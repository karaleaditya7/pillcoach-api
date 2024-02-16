using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_CalcPharmacyPDC_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[CalcPharmacyPDC]
(
	@durationMonths int
	, @reportMonth date = null
	, @pharmacyId int = null
	, @maxRecordCount int = null
)
as
begin
    set nocount on

	if @reportMonth is null set @reportMonth = getdate()

	if day(@reportMonth) != 1 set @reportMonth = dateadd(d, -day(@reportMonth) + 1, @reportMonth)

	if not @durationMonths in (6, 12) set @durationMonths = 6

	if isnull(@maxRecordCount, 0) <= 0 set @maxRecordCount = null

	declare @patients table ( id int identity, patientId int )

	insert @patients select Id from patient p where p.pharmacyId = isnull(@pharmacyId, p.pharmacyId) and p.isDeleted = 0
		and not exists (select * from patientPDC where PatientId = p.Id and PdcMonth = @reportMonth and DurationMonths = @durationMonths)

	declare @id int, @patientId int, @counter int = 0

	select @id = min(id) from @patients

	while not @id is null
	begin
		select @patientId = patientId from @patients where id = @id

		if not exists (select * from patientPDC where PatientId = @patientId and PdcMonth = @reportMonth and DurationMonths = @durationMonths)
		begin
			begin try
				insert patientPDC ( PatientId, PdcMonth, DurationMonths, Condition, PDC, StartDate, EndDate, TotalDays, TotalFills, HasExclusions, Consumptions )
				exec CalcPatientPDC @patientId, @durationMonths, @reportMonth

				set @counter = @counter + 1
			end try
			begin catch
				print error_message()
			end catch
		end

		-- update medication status of this pharmacy
		exec [DeactivateUnusedMedications] @pharmacyId = null, @patientId = @patientId

		select @patientId = null

		if isnull(@maxRecordCount, 0) > 0 and @counter >= @maxRecordCount
			set @id = null
		else
			select @id = min(id) from @patients where id > @id
	end

	set nocount off
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
