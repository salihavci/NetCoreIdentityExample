﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreIdentityExample.Migrations
{
    public partial class initialtwofa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "TwoFactor",
                table: "AspNetUsers",
                type: "smallint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactor",
                table: "AspNetUsers");
        }
    }
}
