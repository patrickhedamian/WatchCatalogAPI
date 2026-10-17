using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WatchCatalogAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialWatchCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CaliberAndMovement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Chronograph = table.Column<bool>(type: "bit", nullable: false),
                    WeightGrams = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    HeightMm = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DiameterMm = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ThicknessMm = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    JewelCount = table.Column<int>(type: "int", nullable: false),
                    CaseMaterial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrapMaterial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchProducts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WatchProducts",
                columns: new[] { "Id", "CaliberAndMovement", "CaseMaterial", "Chronograph", "DiameterMm", "FullDescription", "HeightMm", "ImageUrl", "JewelCount", "Model", "Name", "Price", "ProductNumber", "ShortDescription", "StrapMaterial", "ThicknessMm", "WeightGrams" },
                values: new object[,]
                {
                    { 1, "Automatic, 3 hands", "316L stainless steel", false, 40m, "A brushed steel field watch with a clear dial, luminous hands, and dependable automatic movement.", 11m, "https://placehold.co/640x480?text=Aster%20Field%20Automatic", 24, "Field Automatic", "Aster", 12900m, "WC-1001", "A versatile automatic field watch.", "Canvas", 11m, 82m },
                    { 2, "Quartz chronograph", "Stainless steel", true, 42m, "A balanced chronograph with three subdials, a tachymeter scale, and a comfortable stainless-steel bracelet.", 12m, "https://placehold.co/640x480?text=Meridian%20Chronograph%2042", 0, "Chronograph 42", "Meridian", 18500m, "WC-1002", "A precise everyday chronograph.", "Stainless steel", 12m, 145m },
                    { 3, "Automatic, 3 hands", "316L stainless steel", false, 41m, "A robust dive-style watch with a rotating timing bezel, high-contrast dial, and 200-metre water resistance.", 13m, "https://placehold.co/640x480?text=Northstar%20Diver%20200", 21, "Diver 200", "Northstar", 24750m, "WC-1003", "A bold watch for water and land.", "Rubber", 13m, 168m },
                    { 4, "Automatic, moonphase", "Stainless steel", false, 39m, "A slim dress watch featuring a moonphase display, applied hour markers, and a polished stainless-steel case.", 9m, "https://placehold.co/640x480?text=Solstice%20Moonphase%20Classic", 26, "Moonphase Classic", "Solstice", 32900m, "WC-1004", "A refined dress watch with moonphase.", "Genuine leather", 9m, 74m },
                    { 5, "Automatic GMT", "Titanium", false, 42m, "A legible pilot-inspired GMT watch with a 24-hour hand, oversized crown, and a durable nylon strap.", 12m, "https://placehold.co/640x480?text=Vector%20Pilot%20GMT", 24, "Pilot GMT", "Vector", 21800m, "WC-1005", "Track a second time zone at a glance.", "Nylon", 12m, 91m },
                    { 6, "Manual winding, small seconds", "Stainless steel", false, 38m, "A classic small-seconds watch with a textured dial, slim profile, and versatile brown leather strap.", 8m, "https://placehold.co/640x480?text=Harbor%20Small%20Seconds", 17, "Small Seconds", "Harbor", 16250m, "WC-1006", "A clean dial with a small-seconds display.", "Leather", 8m, 68m },
                    { 7, "Solar quartz, 3 hands", "Resin", false, 40m, "A lightweight sports watch with solar charging, luminous markers, and a scratch-resistant mineral crystal.", 10m, "https://placehold.co/640x480?text=Summit%20Sport%20Solar", 0, "Sport Solar", "Summit", 7950m, "WC-1007", "Low-maintenance solar-powered performance.", "Resin", 10m, 56m },
                    { 8, "Automatic, open-heart", "Stainless steel", false, 40m, "An elegant automatic watch with an open-heart window, exhibition case back, and a textured leather strap.", 11m, "https://placehold.co/640x480?text=Atelier%20Open%20Balance", 20, "Open Balance", "Atelier", 28400m, "WC-1008", "An open-heart view of the movement.", "Leather", 11m, 88m },
                    { 9, "Automatic, 3 hands", "Stainless steel", false, 38m, "A compact diver-inspired model with a unidirectional bezel, luminous display, and a flexible silicone strap.", 12m, "https://placehold.co/640x480?text=Tide%20Coastal%20Diver", 24, "Coastal Diver", "Tide", 13900m, "WC-1009", "A compact diver-inspired daily watch.", "Silicone", 12m, 119m },
                    { 10, "Quartz, 3 hands", "Stainless steel", false, 38m, "A minimalist three-hand watch with a clean dial, slim case, and quick-release mesh bracelet.", 7m, "https://placehold.co/640x480?text=Lumen%20Minimal%2038", 0, "Minimal 38", "Lumen", 9900m, "WC-1010", "A pared-back design for everyday wear.", "Mesh steel", 7m, 96m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchProducts_ProductNumber",
                table: "WatchProducts",
                column: "ProductNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchProducts");
        }
    }
}
