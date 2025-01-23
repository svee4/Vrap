using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vrap.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TableId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataEntries_DataTables_TableId",
                        column: x => x.TableId,
                        principalTable: "DataTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    Ordinal = table.Column<int>(type: "integer", nullable: false),
                    TableId = table.Column<int>(type: "integer", nullable: false),
                    FieldType = table.Column<int>(type: "integer", nullable: false),
                    DateTimeField_MinValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DateTimeField_MaxValue = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MinValue = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxValue = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxLength = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableFields_DataTables_TableId",
                        column: x => x.TableId,
                        principalTable: "DataTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnumOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FieldId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnumOptions_TableFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "TableFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryId = table.Column<int>(type: "integer", nullable: false),
                    TableFieldId = table.Column<int>(type: "integer", nullable: false),
                    FieldType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValueId = table.Column<int>(type: "integer", nullable: true),
                    NumberEntry_Value = table.Column<decimal>(type: "numeric", nullable: true),
                    StringEntry_Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldEntries_DataEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "DataEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldEntries_EnumOptions_ValueId",
                        column: x => x.ValueId,
                        principalTable: "EnumOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldEntries_TableFields_TableFieldId",
                        column: x => x.TableFieldId,
                        principalTable: "TableFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataEntries_TableId",
                table: "DataEntries",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_EnumOptions_FieldId",
                table: "EnumOptions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldEntries_EntryId",
                table: "FieldEntries",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldEntries_TableFieldId",
                table: "FieldEntries",
                column: "TableFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldEntries_ValueId",
                table: "FieldEntries",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_TableFields_TableId",
                table: "TableFields",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldEntries");

            migrationBuilder.DropTable(
                name: "DataEntries");

            migrationBuilder.DropTable(
                name: "EnumOptions");

            migrationBuilder.DropTable(
                name: "TableFields");

            migrationBuilder.DropTable(
                name: "DataTables");
        }
    }
}
