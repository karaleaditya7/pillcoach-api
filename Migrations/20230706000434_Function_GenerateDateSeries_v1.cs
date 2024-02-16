using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Function_GenerateDateSeries_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter function [GenerateDateSeries]
(
	@start_date as date
	, @end_date as date
)
returns @dates table ( date_value date )
as
begin
	;with dates as (
		select [date] = @start_date
		union all select [date] = dateadd(day, 1, [date])
		from dates where date < @end_date
	)
	insert into @dates
	select [date] from dates
    
	return;
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
