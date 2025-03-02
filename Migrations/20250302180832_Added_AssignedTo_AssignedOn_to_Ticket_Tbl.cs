using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSupportXPress.Migrations
{
    /// <inheritdoc />
    public partial class Added_AssignedTo_AssignedOn_to_Ticket_Tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedOn",
                table: "Tickets",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "Tickets",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToId",
                table: "Tickets",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedToId",
                table: "Tickets",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedToId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedToId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Tickets");
        }
    }
}
