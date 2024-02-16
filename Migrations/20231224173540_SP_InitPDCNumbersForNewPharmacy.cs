using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_InitPDCNumbersForNewPharmacy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[InitPDCNumbersForNewPharmacy]
(
	@pharmacyId int
)
as
begin
    set nocount on

	if @pharmacyId > 0
		and exists (
			select * from pharmacy where isDeleted = 0 and Id = @pharmacyId
		)
		and not exists (
			select * from patient p inner join patientPDC pdc
			on p.Id = pdc.PatientId and p.pharmacyId = @pharmacyId
			where p.isDeleted = 0
		)
	begin
		declare @reportMonth date, @today date = getdate()
		set @reportMonth = dateadd(m, -12, @today) -- consider last one year of data

		while @reportMonth <= @today
		begin
			exec CalcPharmacyPDC @pharmacyId = @pharmacyId, @durationMonths = 6, @reportMonth = @reportMonth
			exec CalcPharmacyPDC @pharmacyId = @pharmacyId, @durationMonths = 12, @reportMonth = @reportMonth

			set @reportMonth = dateadd(m, 1, @reportMonth)
		end
	end

	set nocount off
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"drop procedure if exists [InitPDCNumbersForNewPharmacy]";

            migrationBuilder.Sql(sqlScript);
        }
    }
}
