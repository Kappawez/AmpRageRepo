using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AmpRageRepo.Migrations
{
    public partial class initDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    regno = table.Column<string>(nullable: true),
                    vin = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LicensePlate = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    Make = table.Column<string>(nullable: true),
                    Capacity = table.Column<decimal>(nullable: false),
                    ZeroToHundred = table.Column<decimal>(nullable: false),
                    TopSpeed = table.Column<int>(nullable: false),
                    Range = table.Column<int>(nullable: false),
                    Efficiency = table.Column<decimal>(nullable: false),
                    Fastcharge = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Data2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    make = table.Column<string>(nullable: true),
                    model = table.Column<string>(nullable: true),
                    status = table.Column<bool>(nullable: true),
                    color = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    type_class = table.Column<string>(nullable: true),
                    vehicle_year = table.Column<int>(nullable: true),
                    model_year = table.Column<int>(nullable: true),
                    reused_regno = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Data3",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    power_hp_1 = table.Column<decimal>(nullable: true),
                    power_hp_2 = table.Column<decimal>(nullable: true),
                    power_hp_3 = table.Column<decimal>(nullable: true),
                    power_kw_1 = table.Column<decimal>(nullable: true),
                    power_kw_2 = table.Column<decimal>(nullable: true),
                    power_kw_3 = table.Column<decimal>(nullable: true),
                    cylinder_volume = table.Column<decimal>(nullable: true),
                    top_speed = table.Column<decimal>(nullable: true),
                    fuel_1 = table.Column<decimal>(nullable: true),
                    fuel_2 = table.Column<decimal>(nullable: true),
                    fuel_3 = table.Column<decimal>(nullable: true),
                    fuel_combination = table.Column<decimal>(nullable: true),
                    consumption_1 = table.Column<decimal>(nullable: true),
                    consumption_2 = table.Column<decimal>(nullable: true),
                    consumption_3 = table.Column<decimal>(nullable: true),
                    co2_1 = table.Column<decimal>(nullable: true),
                    co2_2 = table.Column<decimal>(nullable: true),
                    co2_3 = table.Column<decimal>(nullable: true),
                    transmission = table.Column<int>(nullable: true),
                    four_wheel_drive = table.Column<bool>(nullable: false),
                    sound_level_1 = table.Column<decimal>(nullable: true),
                    sound_level_2 = table.Column<decimal>(nullable: true),
                    sound_level_3 = table.Column<decimal>(nullable: true),
                    number_of_passengers = table.Column<int>(nullable: false),
                    passenger_airbag = table.Column<bool>(nullable: false),
                    hitch = table.Column<decimal>(nullable: true),
                    hitch_2 = table.Column<decimal>(nullable: true),
                    chassi_code_1 = table.Column<decimal>(nullable: true),
                    chassi_code_2 = table.Column<decimal>(nullable: true),
                    chassi = table.Column<string>(nullable: true),
                    color = table.Column<string>(nullable: true),
                    length = table.Column<decimal>(nullable: true),
                    width = table.Column<decimal>(nullable: true),
                    height = table.Column<decimal>(nullable: true),
                    kerb_weight = table.Column<decimal>(nullable: true),
                    total_weight = table.Column<decimal>(nullable: true),
                    load_weight = table.Column<decimal>(nullable: true),
                    trailer_weight = table.Column<decimal>(nullable: true),
                    unbraked_trailer_weight = table.Column<decimal>(nullable: true),
                    trailer_weight_b = table.Column<decimal>(nullable: true),
                    trailer_weight_be = table.Column<decimal>(nullable: true),
                    carriage_weight = table.Column<decimal>(nullable: true),
                    tire_front = table.Column<decimal>(nullable: true),
                    tire_back = table.Column<decimal>(nullable: true),
                    rim_front = table.Column<string>(nullable: true),
                    rim_back = table.Column<string>(nullable: true),
                    axel_width = table.Column<decimal>(nullable: true),
                    category = table.Column<string>(nullable: true),
                    eeg = table.Column<string>(nullable: true),
                    nox_1 = table.Column<decimal>(nullable: true),
                    nox_2 = table.Column<decimal>(nullable: true),
                    nox_3 = table.Column<decimal>(nullable: true),
                    thc_nox_1 = table.Column<decimal>(nullable: true),
                    thc_nox_2 = table.Column<decimal>(nullable: true),
                    thc_nox_3 = table.Column<decimal>(nullable: true),
                    eco_class = table.Column<int>(nullable: true),
                    emission_class = table.Column<int>(nullable: true),
                    euro_ncap = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    dataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Basic_Data2_dataId",
                        column: x => x.dataId,
                        principalTable: "Data2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Technical",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    dataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technical", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Technical_Data3_dataId",
                        column: x => x.dataId,
                        principalTable: "Data3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(nullable: true),
                    attributesId = table.Column<int>(nullable: true),
                    basicId = table.Column<int>(nullable: true),
                    technicalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Data_Attributes_attributesId",
                        column: x => x.attributesId,
                        principalTable: "Attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Data_Basic_basicId",
                        column: x => x.basicId,
                        principalTable: "Basic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Data_Technical_technicalId",
                        column: x => x.technicalId,
                        principalTable: "Technical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    rel = table.Column<string>(nullable: true),
                    uri = table.Column<string>(nullable: true),
                    DataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Link_Data_DataId",
                        column: x => x.DataId,
                        principalTable: "Data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RootObjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    dataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RootObjects_Data_dataId",
                        column: x => x.dataId,
                        principalTable: "Data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Basic_dataId",
                table: "Basic",
                column: "dataId");

            migrationBuilder.CreateIndex(
                name: "IX_Data_attributesId",
                table: "Data",
                column: "attributesId");

            migrationBuilder.CreateIndex(
                name: "IX_Data_basicId",
                table: "Data",
                column: "basicId");

            migrationBuilder.CreateIndex(
                name: "IX_Data_technicalId",
                table: "Data",
                column: "technicalId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_DataId",
                table: "Link",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_RootObjects_dataId",
                table: "RootObjects",
                column: "dataId");

            migrationBuilder.CreateIndex(
                name: "IX_Technical_dataId",
                table: "Technical",
                column: "dataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "RootObjects");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "Basic");

            migrationBuilder.DropTable(
                name: "Technical");

            migrationBuilder.DropTable(
                name: "Data2");

            migrationBuilder.DropTable(
                name: "Data3");
        }
    }
}
