﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class ss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "comments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}