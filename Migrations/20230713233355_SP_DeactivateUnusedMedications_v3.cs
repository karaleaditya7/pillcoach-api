using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_DeactivateUnusedMedications_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[DeactivateUnusedMedications]
(
	@pharmacyId int = null
	, @patientId int = null
)
as
begin
    set nocount on

	declare @now datetime = getdate(), @maxMonths int = 4

	if isnull(@pharmacyId, 0) > 0
	begin
		begin try
			-- set medication as inactive, if last fill date is older than max months
			update medication set inUse = 0 where inUse = 1
				and (
					datediff(m, lastFillDate, @now) > @maxMonths
					or (datediff(m, lastFillDate, @now) = @maxMonths and day(lastFillDate) <= day(@now))
				)
				and patientId in (select Id from patient where pharmacyId = @pharmacyId)
		end try
		begin catch
		end catch
	end

	else if isnull(@patientId, 0) > 0
	begin
		begin try
			-- set medication as inactive, if last fill date is older than max months
			update medication set inUse = 0 where inUse = 1
				and (
					datediff(m, lastFillDate, @now) > @maxMonths
					or (datediff(m, lastFillDate, @now) = @maxMonths and day(lastFillDate) <= day(@now))
				)
				and patientId = @patientId
		end try
		begin catch
		end catch
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
