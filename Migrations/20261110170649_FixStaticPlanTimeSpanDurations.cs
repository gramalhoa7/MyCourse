using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourseAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixStaticPlanTimeSpanDurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Duration",
                value: new TimeSpan(365, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Duration",
                value: new TimeSpan(180, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 3,
                column: "Duration",
                value: new TimeSpan(30, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Duration",
                value: new TimeSpan(315359999984544));

            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Duration",
                value: new TimeSpan(156383999999957));

            migrationBuilder.UpdateData(
                table: "PlanTimeSpans",
                keyColumn: "Id",
                keyValue: 3,
                column: "Duration",
                value: new TimeSpan(25919999999998));
        }
    }
}
