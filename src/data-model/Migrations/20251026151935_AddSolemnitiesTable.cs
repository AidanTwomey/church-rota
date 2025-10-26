using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace church_rota.data_model.Migrations
{
    /// <inheritdoc />
    public partial class AddSolemnitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Solemnities",
                columns: table => new
                {
                    SolemnityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solemnities", x => x.SolemnityId);
                });

            migrationBuilder.CreateTable(
                name: "PeopleRoles",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeopleRoles", x => new { x.PersonId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_PeopleRoles_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeopleRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_Schedules_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeopleRoles_RoleId",
                table: "PeopleRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PersonId",
                table: "Schedules",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RoleId",
                table: "Schedules",
                column: "RoleId");

            // Seed Solemnities data from CSV
            var now = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Solemnities",
                columns: new[] { "Name", "Date", "Season", "CreatedDate", "ModifiedDate" },
                values: new object[,]
                {
                    { "30th Sunday in Ordinary Time", new DateTime(2025, 10, 26), "Ordinary Time", now, now },
                    { "31st Sunday in Ordinary Time", new DateTime(2025, 11, 2), "Ordinary Time", now, now },
                    { "32nd Sunday in Ordinary Time", new DateTime(2025, 11, 9), "Ordinary Time", now, now },
                    { "33rd Sunday in Ordinary Time", new DateTime(2025, 11, 16), "Ordinary Time", now, now },
                    { "Christ the King (Last Sunday in Ordinary Time)", new DateTime(2025, 11, 23), "Ordinary Time", now, now },
                    { "1st Sunday of Advent", new DateTime(2025, 11, 30), "Advent", now, now },
                    { "2nd Sunday of Advent", new DateTime(2025, 12, 7), "Advent", now, now },
                    { "3rd Sunday of Advent", new DateTime(2025, 12, 14), "Advent", now, now },
                    { "4th Sunday of Advent", new DateTime(2025, 12, 21), "Advent", now, now },
                    { "Christmas - The Nativity of the Lord", new DateTime(2025, 12, 25), "Christmas", now, now },
                    { "The Holy Family of Jesus Mary and Joseph", new DateTime(2025, 12, 28), "Christmas", now, now },
                    { "Solemnity of Mary Mother of God", new DateTime(2026, 1, 1), "Christmas", now, now },
                    { "2nd Sunday after the Nativity", new DateTime(2026, 1, 4), "Christmas", now, now },
                    { "The Baptism of the Lord", new DateTime(2026, 1, 11), "Christmas", now, now },
                    { "2nd Sunday in Ordinary Time", new DateTime(2026, 1, 18), "Ordinary Time", now, now },
                    { "3rd Sunday in Ordinary Time", new DateTime(2026, 1, 25), "Ordinary Time", now, now },
                    { "4th Sunday in Ordinary Time", new DateTime(2026, 2, 1), "Ordinary Time", now, now },
                    { "5th Sunday in Ordinary Time", new DateTime(2026, 2, 8), "Ordinary Time", now, now },
                    { "6th Sunday in Ordinary Time", new DateTime(2026, 2, 15), "Ordinary Time", now, now },
                    { "Ash Wednesday (start of Lent)", new DateTime(2026, 2, 18), "Lent", now, now },
                    { "1st Sunday of Lent", new DateTime(2026, 2, 22), "Lent", now, now },
                    { "2nd Sunday of Lent", new DateTime(2026, 3, 1), "Lent", now, now },
                    { "3rd Sunday of Lent", new DateTime(2026, 3, 8), "Lent", now, now },
                    { "4th Sunday of Lent", new DateTime(2026, 3, 15), "Lent", now, now },
                    { "5th Sunday of Lent", new DateTime(2026, 3, 22), "Lent", now, now },
                    { "Palm Sunday of the Lord's Passion", new DateTime(2026, 3, 29), "Lent/Holy Week", now, now },
                    { "Holy Thursday", new DateTime(2026, 4, 2), "Triduum", now, now },
                    { "Good Friday", new DateTime(2026, 4, 3), "Triduum", now, now },
                    { "Easter Sunday - The Resurrection of the Lord", new DateTime(2026, 4, 5), "Easter", now, now },
                    { "2nd Sunday of Easter (Divine Mercy Sunday)", new DateTime(2026, 4, 12), "Easter", now, now },
                    { "3rd Sunday of Easter", new DateTime(2026, 4, 19), "Easter", now, now },
                    { "4th Sunday of Easter", new DateTime(2026, 4, 26), "Easter", now, now },
                    { "5th Sunday of Easter", new DateTime(2026, 5, 3), "Easter", now, now },
                    { "6th Sunday of Easter", new DateTime(2026, 5, 10), "Easter", now, now },
                    { "The Ascension of the Lord", new DateTime(2026, 5, 14), "Easter", now, now },
                    { "7th Sunday of Easter", new DateTime(2026, 5, 17), "Easter", now, now },
                    { "Pentecost Sunday", new DateTime(2026, 5, 24), "Easter", now, now },
                    { "The Most Holy Trinity", new DateTime(2026, 5, 31), "Ordinary Time", now, now },
                    { "The Most Holy Body and Blood of Christ (Corpus Christi)", new DateTime(2026, 6, 7), "Ordinary Time", now, now },
                    { "11th Sunday in Ordinary Time", new DateTime(2026, 6, 14), "Ordinary Time", now, now },
                    { "12th Sunday in Ordinary Time", new DateTime(2026, 6, 21), "Ordinary Time", now, now },
                    { "Saints Peter and Paul Apostles (Solemnity transferred from June 29)", new DateTime(2026, 6, 28), "Ordinary Time", now, now },
                    { "14th Sunday in Ordinary Time", new DateTime(2026, 7, 5), "Ordinary Time", now, now },
                    { "15th Sunday in Ordinary Time", new DateTime(2026, 7, 12), "Ordinary Time", now, now },
                    { "16th Sunday in Ordinary Time", new DateTime(2026, 7, 19), "Ordinary Time", now, now },
                    { "17th Sunday in Ordinary Time", new DateTime(2026, 7, 26), "Ordinary Time", now, now },
                    { "18th Sunday in Ordinary Time", new DateTime(2026, 8, 2), "Ordinary Time", now, now },
                    { "19th Sunday in Ordinary Time", new DateTime(2026, 8, 9), "Ordinary Time", now, now },
                    { "The Assumption of the Blessed Virgin Mary (Solemnity transferred from August 15)", new DateTime(2026, 8, 16), "Ordinary Time", now, now },
                    { "21st Sunday in Ordinary Time", new DateTime(2026, 8, 23), "Ordinary Time", now, now },
                    { "22nd Sunday in Ordinary Time", new DateTime(2026, 8, 30), "Ordinary Time", now, now },
                    { "23rd Sunday in Ordinary Time", new DateTime(2026, 9, 6), "Ordinary Time", now, now },
                    { "24th Sunday in Ordinary Time", new DateTime(2026, 9, 13), "Ordinary Time", now, now },
                    { "25th Sunday in Ordinary Time", new DateTime(2026, 9, 20), "Ordinary Time", now, now },
                    { "26th Sunday in Ordinary Time", new DateTime(2026, 9, 27), "Ordinary Time", now, now },
                    { "27th Sunday in Ordinary Time", new DateTime(2026, 10, 4), "Ordinary Time", now, now },
                    { "28th Sunday in Ordinary Time", new DateTime(2026, 10, 11), "Ordinary Time", now, now },
                    { "29th Sunday in Ordinary Time", new DateTime(2026, 10, 18), "Ordinary Time", now, now },
                    { "30th Sunday in Ordinary Time", new DateTime(2026, 10, 25), "Ordinary Time", now, now }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeopleRoles");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Solemnities");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
