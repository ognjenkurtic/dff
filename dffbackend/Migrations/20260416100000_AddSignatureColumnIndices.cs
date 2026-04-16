using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dffbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSignatureColumnIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE INDEX IX_Signatures_Signature1 ON Signatures (Signature1(255));");
            migrationBuilder.Sql("CREATE INDEX IX_Signatures_Signature2 ON Signatures (Signature2(255));");
            migrationBuilder.Sql("CREATE INDEX IX_Signatures_Signature3 ON Signatures (Signature3(255));");
            migrationBuilder.Sql("CREATE INDEX IX_Signatures_Signature4 ON Signatures (Signature4(255));");
            migrationBuilder.Sql("CREATE INDEX IX_Signatures_Signature5 ON Signatures (Signature5(255));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Signatures_Signature1", table: "Signatures");
            migrationBuilder.DropIndex(name: "IX_Signatures_Signature2", table: "Signatures");
            migrationBuilder.DropIndex(name: "IX_Signatures_Signature3", table: "Signatures");
            migrationBuilder.DropIndex(name: "IX_Signatures_Signature4", table: "Signatures");
            migrationBuilder.DropIndex(name: "IX_Signatures_Signature5", table: "Signatures");
        }
    }
}
