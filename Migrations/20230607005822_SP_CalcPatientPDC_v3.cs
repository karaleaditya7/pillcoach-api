using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_CalcPatientPDC_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[CalcPatientPDC]
(
	@patientId int
	, @months int
	, @reportMonth date
)
as
begin
    set nocount on

	declare @pdc_numbers table (
		id int identity
		, condition varchar(20)
		, pdc decimal(5, 2)
		, startDate date
		, endDate date
		, totalDays int
		, totalFills int
		, hasExclusions bit
		, consumptions int
	)

	insert @pdc_numbers ( condition ) values('Cholesterol'), ('Diabetes'), ('RASA')

	if day(@reportMonth) != 1 set @reportMonth = dateadd(d, -day(@reportMonth) + 1, @reportMonth)

	declare @startDate date, @endDate date, @totalConsumptionDays int, @totalDays int, @minThresholdDays int = 91

	set @endDate = eomonth(dateadd(m, -2, @reportMonth))
	set @startDate = dateadd(m, -@months, dateadd(d, 1, @endDate))

	if @months = 12
	begin
		set @startDate = convert(date, formatmessage('1/1/%i', year(@endDate)))

		if month(@endDate) = 3 set @minThresholdDays = 90 -- min days is 90 for range Jan-Mar, as Feb mostly has 28 days and sum of these three months is usually 90 days
	end

	declare @id int, @lastRefillDate date, @totalFills int, @pdc decimal(5, 2) = 0, @condition varchar(20), @hasExclusions bit

	select @id = min(id) from @pdc_numbers

	while not @id is null
	begin
		select @condition = condition from @pdc_numbers where id = @id

		if @months = 6 set @startDate = dateadd(m, -@months, dateadd(d, 1, @endDate))
		else set @startDate = convert(date, formatmessage('1/1/%i', year(@endDate)))

		select @lastRefillDate = null, @totalFills = 0, @pdc = 0, @totalConsumptionDays = 0, @totalDays = 0, @hasExclusions = 0

		select @lastRefillDate = min(lastFillDate), @totalFills = count(distinct lastFillDate) from medication
		where patientId = @patientId and condition = @condition and lastFillDate between @startDate and @endDate

		if not exists (
			-- ensure that there are no exclusions
			select * from Pdc_Medications where category in ('Insulins', 'Sacubitril_Valsartan') and code in (
				select distinct ndcNumber from medication where patientId = @patientId and lastFillDate between @startDate and @endDate
					and condition in (@condition, 
						case @condition
							when 'Diabetes' then 'Insulins'
							when 'RASA' then 'Sacubitril_Valsartan'
							else '-' 
						end
					)
			)
		)
		begin
			if @totalFills >= 2 and not @lastRefillDate is null
			begin
				if @lastRefillDate > @startDate set @startDate = @lastRefillDate

				set @totalDays = datediff(d, @startDate, @endDate) + 1 -- +1 to include both start and end dates

				if @totalDays >= @minThresholdDays
				begin
					select @totalConsumptionDays = count(*) from medicationConsumptions
					where patientId = @patientId and condition = @condition and [status] = 1 and convert(date, [date]) between @startDate and @endDate

					if @totalConsumptionDays > 0 set @pdc = ((@totalConsumptionDays * 1.0) / @totalDays) * 100
				end
			end
		end
		else
		begin
			set @hasExclusions = 1
		end

		if @totalDays = 0 set @totalDays = datediff(d, @startDate, @endDate) + 1

		update @pdc_numbers set
			pdc = @pdc
			, startDate = @startDate
			, endDate = @endDate
			, totalDays = isnull(@totalDays, 0)
			, totalFills = @totalFills
			, hasExclusions = @hasExclusions
			, consumptions = isnull(@totalConsumptionDays, 0)
		where id = @id

		select @id = min(id) from @pdc_numbers where id > @id
	end

	select @patientId, @reportMonth, @months, condition, pdc, startDate, endDate, totalDays, totalFills, hasExclusions, consumptions from @pdc_numbers

	set nocount off
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
