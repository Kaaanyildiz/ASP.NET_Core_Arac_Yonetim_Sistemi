using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace identityApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "Color", "Description", "EngineSize", "ImageUrl", "IsAvailable", "IsDeleted", "Mileage", "Model", "Price", "Year" },
                values: new object[,]
                {
                    { 1, "Mercedes", "Siyah", "Hatasız boyasız Mercedes", "1.6", null, true, false, 15000, "C180", 850000m, 2020 },
                    { 2, "BMW", "Beyaz", "Tam donanım BMW", "2.0", null, true, false, 25000, "320i", 780000m, 2019 },
                    { 3, "Audi", "Gri", "Audi A4 Quattro", "2.0", null, true, false, 10000, "A4", 900000m, 2021 }
                });
        }
    }
}
