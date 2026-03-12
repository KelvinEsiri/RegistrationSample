using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistrationSample.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaritalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "AspNetUsers");
        }
    }
}
