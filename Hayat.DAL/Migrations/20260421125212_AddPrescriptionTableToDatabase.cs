using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hayat.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionTableToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistories_Doctors_DoctorId",
                table: "MedicalHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories");

            migrationBuilder.RenameTable(
                name: "MedicalHistories",
                newName: "VisitsHistories");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "VisitsHistories",
                newName: "IX_VisitsHistories_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalHistories_DoctorId",
                table: "VisitsHistories",
                newName: "IX_VisitsHistories_DoctorId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "VisitsHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VisitsHistories",
                table: "VisitsHistories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DrugName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    VisitHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_VisitsHistories_VisitHistoryId",
                        column: x => x.VisitHistoryId,
                        principalTable: "VisitsHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_VisitHistoryId",
                table: "Prescriptions",
                column: "VisitHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitsHistories_Doctors_DoctorId",
                table: "VisitsHistories",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitsHistories_Patients_PatientId",
                table: "VisitsHistories",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitsHistories_Doctors_DoctorId",
                table: "VisitsHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitsHistories_Patients_PatientId",
                table: "VisitsHistories");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VisitsHistories",
                table: "VisitsHistories");

            migrationBuilder.RenameTable(
                name: "VisitsHistories",
                newName: "MedicalHistories");

            migrationBuilder.RenameIndex(
                name: "IX_VisitsHistories_PatientId",
                table: "MedicalHistories",
                newName: "IX_MedicalHistories_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitsHistories_DoctorId",
                table: "MedicalHistories",
                newName: "IX_MedicalHistories_DoctorId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistories_Doctors_DoctorId",
                table: "MedicalHistories",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistories_Patients_PatientId",
                table: "MedicalHistories",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
