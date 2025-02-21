using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameZone.Repository.Migrations
{
    /// <inheritdoc />
    public partial class cover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "AspNetUsers",
                newName: "Cover");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cover",
                table: "AspNetUsers",
                newName: "Image");
        }
    }
}
