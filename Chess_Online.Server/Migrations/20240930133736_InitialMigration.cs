using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chess_Online.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChessBoard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CB_8A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_8H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_7H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_6H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_5H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_4H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_3H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_2H = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1E = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1F = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1G = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CB_1H = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessBoard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JwtTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jti = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameInstancesEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChessBoardId = table.Column<int>(type: "int", nullable: false),
                    PlayerTeamWhite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerTeamBlack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckByWhite = table.Column<int>(type: "int", nullable: false),
                    CheckByBlack = table.Column<int>(type: "int", nullable: false),
                    GameEnded = table.Column<bool>(type: "bit", nullable: false),
                    PlayerTurn = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInstancesEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameInstancesEntity_ChessBoard_ChessBoardId",
                        column: x => x.ChessBoardId,
                        principalTable: "ChessBoard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameInstancesEntity_ChessBoardId",
                table: "GameInstancesEntity",
                column: "ChessBoardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "GameInstancesEntity");

            migrationBuilder.DropTable(
                name: "JwtTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ChessBoard");
        }
    }
}
