using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class SeedIdentityTables : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
				values: new object[,]
				{
					{ "01965d90-71b8-72b4-a99f-de3bf84d13de", "01965d90-71b8-72b4-a99f-de3c75cefcf7", false, false, "Admin", "ADMIN" },
					{ "01965d90-71b8-72b4-a99f-de3d048430e9", "01965d90-71b8-72b4-a99f-de3e7ed6cc0c", true, false, "Member", "MEMBER" }
				});

			migrationBuilder.InsertData(
				table: "AspNetUsers",
				columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
				values: new object[] { "01965d90-71b8-72b4-a99f-de3822dece9b", 0, "01965d90-71b8-72b4-a99f-de3983d1dd83", "Admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEIwo2J7iINMV3BMBavwBUGZT09tgnaAjUSeoCH9USDE9u4uQ8sYklAFbxhwNRZdH8A==", null, false, "96923A823048450C8D534A5B8C09549F", false, "Admin@survey-basket.com" });

			migrationBuilder.InsertData(
				table: "AspNetRoleClaims",
				columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
				values: new object[,]
				{
					{ 1, "permissions", "polls:read", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 2, "permissions", "polls:add", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 3, "permissions", "polls:update", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 4, "permissions", "polls:delete", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 5, "permissions", "questions:read", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 6, "permissions", "questions:add", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 7, "permissions", "questions:update", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 8, "permissions", "users:read", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 9, "permissions", "users:add", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 10, "permissions", "users:update", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 11, "permissions", "roles:read", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 12, "permissions", "roles:add", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 13, "permissions", "roles:update", "01965d90-71b8-72b4-a99f-de3bf84d13de" },
					{ 14, "permissions", "results:read", "01965d90-71b8-72b4-a99f-de3bf84d13de" }
				});

			migrationBuilder.InsertData(
				table: "AspNetUserRoles",
				columns: new[] { "RoleId", "UserId" },
				values: new object[] { "01965d90-71b8-72b4-a99f-de3bf84d13de", "01965d90-71b8-72b4-a99f-de3822dece9b" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 1);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 2);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 3);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 4);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 5);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 6);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 7);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 8);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 9);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 10);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 11);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 12);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 13);

			migrationBuilder.DeleteData(
				table: "AspNetRoleClaims",
				keyColumn: "Id",
				keyValue: 14);

			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Id",
				keyValue: "01965d90-71b8-72b4-a99f-de3d048430e9");

			migrationBuilder.DeleteData(
				table: "AspNetUserRoles",
				keyColumns: new[] { "RoleId", "UserId" },
				keyValues: new object[] { "01965d90-71b8-72b4-a99f-de3bf84d13de", "01965d90-71b8-72b4-a99f-de3822dece9b" });

			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Id",
				keyValue: "01965d90-71b8-72b4-a99f-de3bf84d13de");

			migrationBuilder.DeleteData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "01965d90-71b8-72b4-a99f-de3822dece9b");
		}
	}
}
