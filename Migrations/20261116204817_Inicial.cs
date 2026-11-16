using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyCourseAPI.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    EducationalLevel = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Objective = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanTimeSpans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTimeSpans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "Settings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DarkTheme = table.Column<bool>(type: "bit", nullable: true),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: true),
                    AutoPlayNextVideo = table.Column<bool>(type: "bit", nullable: true),
                    TextSize = table.Column<int>(type: "int", nullable: false),
                    HighContrast = table.Column<bool>(type: "bit", nullable: true),
                    ReduceMotion = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Settings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlanTimeSpanId = table.Column<int>(type: "int", nullable: false),
                    TestTime = table.Column<long>(type: "bigint", nullable: false),
                    Highlight = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plans_PlanTimeSpans_PlanTimeSpanId",
                        column: x => x.PlanTimeSpanId,
                        principalTable: "PlanTimeSpans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alternatives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letter = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alternatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alternatives_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PricePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentAnswers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentAnswers_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Matter", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Matemática Básica" },
                    { 2, 1, "Aritmética" },
                    { 3, 1, "Álgebra" },
                    { 4, 1, "Geometria" },
                    { 5, 1, "Trigonometria" },
                    { 6, 1, "Geometria Analítica" },
                    { 7, 1, "Combinatória e Probabilidade" },
                    { 8, 1, "Matrizes e Sistemas" },
                    { 9, 2, "Mecânica Básica" },
                    { 10, 2, "Mecânica" },
                    { 11, 2, "Termodinâmica" },
                    { 12, 2, "Óptica" },
                    { 13, 2, "Ondas" },
                    { 14, 2, "Eletricidade" },
                    { 15, 2, "Física Moderna" }
                });

            migrationBuilder.InsertData(
                table: "PlanTimeSpans",
                columns: new[] { "Id", "Duration", "Name" },
                values: new object[,]
                {
                    { 1, 315360000000000L, "Extensivo" },
                    { 2, 155520000000000L, "Intensivo" },
                    { 3, 25920000000000L, "Mensal" },
                    { 4, 9223372036854775807L, "Vitalício" }
                });

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "BranchId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Frações e Equações" },
                    { 2, 1, "Fatoração e Manipulações Algébricas" },
                    { 3, 1, "Potenciação e Radiciação" },
                    { 4, 1, "Razão e Proporção" },
                    { 5, 1, "Equações e Problemas" },
                    { 6, 2, "Divisibilidade, MDC e MMC" },
                    { 7, 2, "PIF: Princípio da Indução Finita" },
                    { 8, 2, "Aritmética Modular" },
                    { 9, 2, "Bases de Numeração" },
                    { 10, 2, "Teoria de Conjuntos" },
                    { 11, 2, "Conjuntos Numéricos" },
                    { 12, 2, "Raciocínio Lógico" },
                    { 13, 2, "Porcentagem" },
                    { 14, 2, "Juros" },
                    { 15, 2, "Estatística" },
                    { 16, 2, "Desigualdade das Médias" },
                    { 17, 3, "Intuição e Definições" },
                    { 18, 3, "Função Afim" },
                    { 19, 3, "Função Quadrática" },
                    { 20, 3, "Função Modular" },
                    { 21, 3, "Outras Funções Clássicas" },
                    { 22, 3, "Função Composta e Inversa" },
                    { 23, 3, "Função Exponencial" },
                    { 24, 3, "Logaritmos e Propriedades" },
                    { 25, 3, "Função Logarítmica" },
                    { 26, 3, "Equações e Inequações Logarítmicas" },
                    { 27, 3, "Progressão Aritmética" },
                    { 28, 3, "Progressão Geométrica" },
                    { 29, 3, "Progressão Harmônica" },
                    { 30, 3, "Binômio de Newton" },
                    { 31, 3, "Unidade Imaginária" },
                    { 32, 3, "Forma Algébrica do Número Complexo" },
                    { 33, 3, "Forma Trigonométrica" },
                    { 34, 3, "Lugares Geométricos" },
                    { 35, 3, "Introdução aos Polinômios" },
                    { 36, 3, "Operações com Polinômios" },
                    { 37, 3, "Equações Algébricas" },
                    { 38, 3, "Aprofundamento" },
                    { 40, 4, "Ângulos e Conceitos Iniciais" },
                    { 41, 4, "Semelhança e Teorema de Tales" },
                    { 42, 4, "Triângulo Retângulo" },
                    { 43, 4, "Circunferência" },
                    { 44, 4, "Triângulos e Pontos Notáveis" },
                    { 45, 4, "Polígonos" },
                    { 46, 4, "Quadriláteros" },
                    { 47, 4, "Trigonometria em Triângulos Quaisquer" },
                    { 48, 4, "Área de Polígonos" },
                    { 49, 4, "Área de Círculo" },
                    { 50, 4, "Geometria de posição" },
                    { 51, 4, "Prismas" },
                    { 52, 4, "Pirâmides" },
                    { 53, 4, "Cilindros" },
                    { 54, 4, "Cones" },
                    { 55, 4, "Esferas" },
                    { 56, 4, "Poliedros" },
                    { 57, 4, "Inscrição de Sólidos" },
                    { 58, 4, "Sólidos de Revolução" },
                    { 59, 5, "Arcos e Ângulos" },
                    { 60, 5, "Trigonometria no Triângulo Retângulo" },
                    { 61, 5, "Ciclo Trigonométrico" },
                    { 62, 5, "Introdução às Equações Trigonométricas" },
                    { 63, 5, "Arcos Compostos" },
                    { 64, 5, "Transformações Trigonométricas" },
                    { 65, 5, "Equações Trigonométricas Avançadas" },
                    { 66, 5, "Inequações Trigonométricas" },
                    { 67, 5, "Funções Trigonométricas" },
                    { 68, 5, "Funções Trigonométricas Inversas" },
                    { 69, 5, "Desigualdades Trigonométricas" },
                    { 70, 6, "Pontos e Coordenadas" },
                    { 71, 6, "Retas" },
                    { 72, 6, "Cônicas" },
                    { 73, 6, "Lugares Geométricos" },
                    { 74, 6, "Geometria Analítica e Álgebra Linear" },
                    { 75, 7, "Princípio Fundamental da Contagem" },
                    { 76, 7, "Fatorial e Permutações" },
                    { 77, 7, "Arranjos e Combinações" },
                    { 78, 7, "Aprofundamento em Combinatória" },
                    { 79, 7, "Teoria das Probabilidades" },
                    { 80, 7, "Probabilidade Condicional" },
                    { 81, 8, "Matrizes" },
                    { 82, 8, "Determinantes" },
                    { 83, 8, "Sistemas Lineares" },
                    { 84, 9, "Fundamentos de Mecânica" },
                    { 85, 9, "Vetores" },
                    { 86, 10, "Introdução à Cinemática" },
                    { 87, 10, "Movimento Retilíneo Uniforme" },
                    { 88, 10, "Movimento Retilíneo Uniformemente Variado" },
                    { 89, 10, "Análise Gráfica" },
                    { 90, 10, "Cinemática Vetorial" },
                    { 91, 10, "Lançamentos" },
                    { 92, 10, "Movimento Circular" },
                    { 93, 10, "Leis de Newton" },
                    { 94, 10, "Atrito e Resistência do Ar" },
                    { 95, 10, "Dinâmica do movimento circular" },
                    { 96, 10, "Trabalho e Energia" },
                    { 97, 10, "Impulso e Quantidade de Movimento" },
                    { 98, 10, "Colisões" },
                    { 99, 10, "Equilíbrio de Corpo Extenso" },
                    { 100, 10, "Leis de Kepler" },
                    { 101, 10, "Gravitação" },
                    { 102, 10, "Hidrostática" },
                    { 103, 10, "Hidrodinâmica" },
                    { 104, 10, "Análise Dimensional" },
                    { 105, 11, "Temperatura" },
                    { 106, 11, "Propagação do Calor" },
                    { 107, 11, "Dilatometria" },
                    { 108, 11, "Calorimetria" },
                    { 109, 11, "Calorimetria (Aprofundamento)" },
                    { 110, 11, "Gases" },
                    { 111, 11, "Lei Zero da Termodinâmica" },
                    { 112, 11, "1ª Lei da Termodinâmica" },
                    { 113, 11, "2ª Lei da Termodinâmica" },
                    { 114, 12, "Fundamentos da Óptica Geométrica" },
                    { 115, 12, "Espelhos Planos" },
                    { 116, 12, "Espelhos Esféricos" },
                    { 117, 12, "Refração" },
                    { 118, 12, "Lentes" },
                    { 119, 12, "Instrumentos Ópticos e Olho Humano" },
                    { 120, 13, "Movimento Harmônico Simples (MHS)" },
                    { 121, 13, "Introdução à Ondulatória" },
                    { 122, 13, "Fenômenos Ondulatórios" },
                    { 123, 13, "Ondas Estacionárias" },
                    { 124, 13, "Acústica" },
                    { 125, 14, "Carga Elétrica e Processos de Eletrização" },
                    { 126, 14, "Força Elétrica" },
                    { 127, 14, "Campo Elétrico" },
                    { 128, 14, "Potencial Elétrico" },
                    { 129, 14, "Condutores" },
                    { 130, 14, "Corrente Elétrica" },
                    { 131, 14, "Lei de Ohm e Resistores" },
                    { 132, 14, "Associação de Resistores" },
                    { 133, 14, "Instrumentos de Medida Elétrica" },
                    { 134, 14, "Geradores e Receptores" },
                    { 135, 14, "Capacitores" },
                    { 136, 14, "Campo Magnético" },
                    { 137, 14, "Força Magnética" },
                    { 138, 14, "Fontes de Campo Magnético" },
                    { 139, 14, "Indução Eletromagnética" },
                    { 140, 15, "Física Quântica" },
                    { 141, 15, "Efeito Fotoelétrico e Comportamento Dual" },
                    { 142, 15, "Relatividade Especial" },
                    { 143, 15, "Física Nuclear" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alternatives_QuestionId",
                table: "Alternatives",
                column: "QuestionId");

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
                name: "IX_CommentAnswers_CommentId",
                table: "CommentAnswers",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentAnswers_UserId",
                table: "CommentAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VideoId",
                table: "Comments",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PlanId",
                table: "Contracts",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_UserId",
                table: "Contracts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_VideoId",
                table: "Notes",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_PlanTimeSpanId",
                table: "Plans",
                column: "PlanTimeSpanId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_BranchId",
                table: "Topics",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TopicId",
                table: "Videos",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alternatives");

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
                name: "CommentAnswers");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "PlanTimeSpans");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
