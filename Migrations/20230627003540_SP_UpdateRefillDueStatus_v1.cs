using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_UpdateRefillDueStatus_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[UpdateRefillDueStatus]
(
	@batchSize int
	, @updateCount int output
)
as
begin
    set nocount on

	set @updateCount = 0

	declare @updatedPatients table ( patientId int, condition varchar(100) )

	declare @today date = getdate()

	update top(@batchSize) medication set
		refillDue = 1
	output
		inserted.patientId, inserted.condition
		into @updatedPatients ( patientId, condition )
	where
		refillDue = 0
		and Id in (
			select MedicationId from RefillDueActivationQueue
			where ActivationDate = @today
		)

	select @updateCount = count(*) from @updatedPatients
	
	if @updateCount > 0
	begin
		update p set
			diabetesRefillDue = case when l.condition = 'Diabetes' then 1 else diabetesRefillDue end
			, cholesterolRefillDue = case when l.condition = 'Cholesterol' then 1 else cholesterolRefillDue end
			, rasaRefillDue = case when l.condition = 'RASA' then 1 else rasaRefillDue end
		from
			patient p
		inner join
			@updatedPatients l
				on l.patientId = p.Id
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
