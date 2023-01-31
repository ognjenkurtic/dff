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
            migrationBuilder.AddColumn<string>(
                name: "Signature5",
                table: "Signatures",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature5",
                table: "Signatures");
        }
    }
}
