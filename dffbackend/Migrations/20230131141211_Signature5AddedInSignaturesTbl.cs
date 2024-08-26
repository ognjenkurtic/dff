using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dffbackend.Migrations
{
    /// <inheritdoc />
    public partial class Signature5AddedInSignaturesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Signatures",
                type: "DateTime",
                nullable: false)
                .Annotation("MySql:Index", "CreationDateIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Signatures");
        }
    }
}
