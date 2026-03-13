using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "ImgId",
                keyValue: 1);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageData",
                table: "Images",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "password_here");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Images");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageData",
                table: "Images",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "ImgId", "Description", "ImageData", "Title", "UploadDate", "UserId" },
                values: new object[] { 1, "Опис тестового фото", null, "Моє перше фото", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "hashed_password_here");
        }
    }
}
