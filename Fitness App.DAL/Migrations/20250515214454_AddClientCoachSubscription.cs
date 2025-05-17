using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_App.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClientCoachSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "Coaches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coaches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClientCoachSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CoachId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    WorkoutPlanId = table.Column<int>(type: "int", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CoachId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCoachSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCoachSubscriptions_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientCoachSubscriptions_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCoachSubscriptions_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCoachSubscriptions_Coaches_CoachId1",
                        column: x => x.CoachId1,
                        principalTable: "Coaches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientCoachSubscriptions_WorkoutPlans_WorkoutPlanId",
                        column: x => x.WorkoutPlanId,
                        principalTable: "WorkoutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientCoachSubscriptions_ApplicationUserId",
                table: "ClientCoachSubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCoachSubscriptions_ClientId",
                table: "ClientCoachSubscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCoachSubscriptions_CoachId",
                table: "ClientCoachSubscriptions",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCoachSubscriptions_CoachId1",
                table: "ClientCoachSubscriptions",
                column: "CoachId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCoachSubscriptions_WorkoutPlanId",
                table: "ClientCoachSubscriptions",
                column: "WorkoutPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientCoachSubscriptions");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coaches");
        }
    }
}
