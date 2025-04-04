using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSupportXPress.Migrations
{
    /// <inheritdoc />
    public partial class Added_Attachment_to_Ticket_Tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "Tickets",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "Tickets");
        }
    }
}
