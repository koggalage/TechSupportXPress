using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSupportXPress.Migrations
{
    /// <inheritdoc />
    public partial class Added_SubCategory_to_Tickets_Tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Tickets",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Tickets",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Tickets",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Tickets",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Comments",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Comments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Comments",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Comments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DeletedById",
                table: "Tickets",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ModifiedById",
                table: "Tickets",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubCategoryId",
                table: "Tickets",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DeletedById",
                table: "Comments",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ModifiedById",
                table: "Comments",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_DeletedById",
                table: "Comments",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ModifiedById",
                table: "Comments",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_DeletedById",
                table: "Tickets",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ModifiedById",
                table: "Tickets",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketSubCategories_SubCategoryId",
                table: "Tickets",
                column: "SubCategoryId",
                principalTable: "TicketSubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_DeletedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ModifiedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_DeletedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ModifiedById",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketSubCategories_SubCategoryId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DeletedById",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ModifiedById",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SubCategoryId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DeletedById",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ModifiedById",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Comments");
        }
    }
}
