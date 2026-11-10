using Microsoft.EntityFrameworkCore;
using MyCourse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Data
{
    public class DBContext : IdentityDbContext<Users, IdentityRole<Guid>, Guid>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<Branches> Branches { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<Videos> Videos { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Plans> Plans { get; set; }
        public DbSet<PlanTimeSpans> PlanTimeSpans { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Alternatives> Alternatives { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Branches>().HasData(
                //MATEMÁTICA
                new Branches { Id = 1, Name = "Matemática Básica", Matter = Matters.Matemática },
                new Branches { Id = 2, Name = "Aritmética", Matter = Matters.Matemática },
                new Branches { Id = 3, Name = "Álgebra", Matter = Matters.Matemática },
                new Branches { Id = 4, Name = "Geometria", Matter = Matters.Matemática },
                new Branches { Id = 5, Name = "Trigonometria", Matter = Matters.Matemática },
                new Branches { Id = 6, Name = "Geometria Analítica", Matter = Matters.Matemática },
                new Branches { Id = 7, Name = "Combinatória e Probabilidade", Matter = Matters.Matemática },
                new Branches { Id = 8, Name = "Matrizes e Sistemas", Matter = Matters.Matemática },

                //FÍSICA
                new Branches { Id = 9, Name = "Mecânica Básica", Matter = Matters.Física },
                new Branches { Id = 10, Name = "Mecânica", Matter = Matters.Física },
                new Branches { Id = 11, Name = "Termodinâmica", Matter = Matters.Física },
                new Branches { Id = 12, Name = "Óptica", Matter = Matters.Física },
                new Branches { Id = 13, Name = "Ondas", Matter = Matters.Física },
                new Branches { Id = 14, Name = "Eletricidade", Matter = Matters.Física },
                new Branches { Id = 15, Name = "Física Moderna", Matter = Matters.Física }
            );
            modelBuilder.Entity<Topics>().HasData(
                // MATEMÁTICA BÁSICA
                new Topics { Id = 1, Name = "Frações e Equações", BranchId = 1 },
                new Topics { Id = 2, Name = "Fatoração e Manipulações Algébricas", BranchId = 1 },
                new Topics { Id = 3, Name = "Potenciação e Radiciação", BranchId = 1 },
                new Topics { Id = 4, Name = "Razão e Proporção", BranchId = 1 },
                new Topics { Id = 5, Name = "Equações e Problemas", BranchId = 1 },

                // ARITMÉTICA
                new Topics { Id = 6, Name = "Divisibilidade, MDC e MMC", BranchId = 2 },
                new Topics { Id = 7, Name = "PIF: Princípio da Indução Finita", BranchId = 2 },
                new Topics { Id = 8, Name = "Aritmética Modular", BranchId = 2 },
                new Topics { Id = 9, Name = "Bases de Numeração", BranchId = 2 },
                new Topics { Id = 10, Name = "Teoria de Conjuntos", BranchId = 2 },
                new Topics { Id = 11, Name = "Conjuntos Numéricos", BranchId = 2 },
                new Topics { Id = 12, Name = "Raciocínio Lógico", BranchId = 2 },
                new Topics { Id = 13, Name = "Porcentagem", BranchId = 2 },
                new Topics { Id = 14, Name = "Juros", BranchId = 2 },
                new Topics { Id = 15, Name = "Estatística", BranchId = 2 },
                new Topics { Id = 16, Name = "Desigualdade das Médias", BranchId = 2 },

                // ÁLGEBRA
                new Topics { Id = 17, Name = "Intuição e Definições", BranchId = 3 },
                new Topics { Id = 18, Name = "Função Afim", BranchId = 3 },
                new Topics { Id = 19, Name = "Função Quadrática", BranchId = 3 },
                new Topics { Id = 20, Name = "Função Modular", BranchId = 3 },
                new Topics { Id = 21, Name = "Outras Funções Clássicas", BranchId = 3 },
                new Topics { Id = 22, Name = "Função Composta e Inversa", BranchId = 3 },
                new Topics { Id = 23, Name = "Função Exponencial", BranchId = 3 },
                new Topics { Id = 24, Name = "Logaritmos e Propriedades", BranchId = 3 },
                new Topics { Id = 25, Name = "Função Logarítmica", BranchId = 3 },
                new Topics { Id = 26, Name = "Equações e Inequações Logarítmicas", BranchId = 3 },
                new Topics { Id = 27, Name = "Progressão Aritmética", BranchId = 3 },
                new Topics { Id = 28, Name = "Progressão Geométrica", BranchId = 3 },
                new Topics { Id = 29, Name = "Progressão Harmônica", BranchId = 3 },
                new Topics { Id = 30, Name = "Binômio de Newton", BranchId = 3 },
                new Topics { Id = 31, Name = "Unidade Imaginária", BranchId = 3 },
                new Topics { Id = 32, Name = "Forma Algébrica do Número Complexo", BranchId = 3 },
                new Topics { Id = 33, Name = "Forma Trigonométrica", BranchId = 3 },
                new Topics { Id = 34, Name = "Lugares Geométricos", BranchId = 3 },
                new Topics { Id = 35, Name = "Introdução aos Polinômios", BranchId = 3 },
                new Topics { Id = 36, Name = "Operações com Polinômios", BranchId = 3 },
                new Topics { Id = 37, Name = "Equações Algébricas", BranchId = 3 },
                new Topics { Id = 38, Name = "Aprofundamento", BranchId = 3 },

                // GEOMETRIA
                new Topics { Id = 40, Name = "Ângulos e Conceitos Iniciais", BranchId = 4 },
                new Topics { Id = 41, Name = "Semelhança e Teorema de Tales", BranchId = 4 },
                new Topics { Id = 42, Name = "Triângulo Retângulo", BranchId = 4 },
                new Topics { Id = 43, Name = "Circunferência", BranchId = 4 },
                new Topics { Id = 44, Name = "Triângulos e Pontos Notáveis", BranchId = 4 },
                new Topics { Id = 45, Name = "Polígonos", BranchId = 4 },
                new Topics { Id = 46, Name = "Quadriláteros", BranchId = 4 },
                new Topics { Id = 47, Name = "Trigonometria em Triângulos Quaisquer", BranchId = 4 },
                new Topics { Id = 48, Name = "Área de Polígonos", BranchId = 4 },
                new Topics { Id = 49, Name = "Área de Círculo", BranchId = 4 },
                new Topics { Id = 50, Name = "Geometria de posição", BranchId = 4 },
                new Topics { Id = 51, Name = "Prismas", BranchId = 4 },
                new Topics { Id = 52, Name = "Pirâmides", BranchId = 4 },
                new Topics { Id = 53, Name = "Cilindros", BranchId = 4 },
                new Topics { Id = 54, Name = "Cones", BranchId = 4 },
                new Topics { Id = 55, Name = "Esferas", BranchId = 4 },
                new Topics { Id = 56, Name = "Poliedros", BranchId = 4 },
                new Topics { Id = 57, Name = "Inscrição de Sólidos", BranchId = 4 },
                new Topics { Id = 58, Name = "Sólidos de Revolução", BranchId = 4 },

                // TRIGONOMETRIA
                new Topics { Id = 59, Name = "Arcos e Ângulos", BranchId = 5 },
                new Topics { Id = 60, Name = "Trigonometria no Triângulo Retângulo", BranchId = 5 },
                new Topics { Id = 61, Name = "Ciclo Trigonométrico", BranchId = 5 },
                new Topics { Id = 62, Name = "Introdução às Equações Trigonométricas", BranchId = 5 },
                new Topics { Id = 63, Name = "Arcos Compostos", BranchId = 5 },
                new Topics { Id = 64, Name = "Transformações Trigonométricas", BranchId = 5 },
                new Topics { Id = 65, Name = "Equações Trigonométricas Avançadas", BranchId = 5 },
                new Topics { Id = 66, Name = "Inequações Trigonométricas", BranchId = 5 },
                new Topics { Id = 67, Name = "Funções Trigonométricas", BranchId = 5 },
                new Topics { Id = 68, Name = "Funções Trigonométricas Inversas", BranchId = 5 },
                new Topics { Id = 69, Name = "Desigualdades Trigonométricas", BranchId = 5 },

                // GEOMETRIA ANALÍTICA
                new Topics { Id = 70, Name = "Pontos e Coordenadas", BranchId = 6 },
                new Topics { Id = 71, Name = "Retas", BranchId = 6 },
                new Topics { Id = 72, Name = "Cônicas", BranchId = 6 },
                new Topics { Id = 73, Name = "Lugares Geométricos", BranchId = 6 },
                new Topics { Id = 74, Name = "Geometria Analítica e Álgebra Linear", BranchId = 6 },

                // COMBINATÓRIA E PROBABILIDADE
                new Topics { Id = 75, Name = "Princípio Fundamental da Contagem", BranchId = 7 },
                new Topics { Id = 76, Name = "Fatorial e Permutações", BranchId = 7 },
                new Topics { Id = 77, Name = "Arranjos e Combinações", BranchId = 7 },
                new Topics { Id = 78, Name = "Aprofundamento em Combinatória", BranchId = 7 },
                new Topics { Id = 79, Name = "Teoria das Probabilidades", BranchId = 7 },
                new Topics { Id = 80, Name = "Probabilidade Condicional", BranchId = 7 },

                // MATRIZES E SISTEMAS
                new Topics { Id = 81, Name = "Matrizes", BranchId = 8 },
                new Topics { Id = 82, Name = "Determinantes", BranchId = 8 },
                new Topics { Id = 83, Name = "Sistemas Lineares", BranchId = 8 },

                //FÍSICA

                // MECÂNICA BÁSICA
                new Topics { Id = 84, Name = "Fundamentos de Mecânica", BranchId = 9 },
                new Topics { Id = 85, Name = "Vetores", BranchId = 9 },

                // MECÂNICA
                new Topics { Id = 86, Name = "Introdução à Cinemática", BranchId = 10 },
                new Topics { Id = 87, Name = "Movimento Retilíneo Uniforme", BranchId = 10 },
                new Topics { Id = 88, Name = "Movimento Retilíneo Uniformemente Variado", BranchId = 10 },
                new Topics { Id = 89, Name = "Análise Gráfica", BranchId = 10 },
                new Topics { Id = 90, Name = "Cinemática Vetorial", BranchId = 10 },
                new Topics { Id = 91, Name = "Lançamentos", BranchId = 10 },
                new Topics { Id = 92, Name = "Movimento Circular", BranchId = 10 },
                new Topics { Id = 93, Name = "Leis de Newton", BranchId = 10 },
                new Topics { Id = 94, Name = "Atrito e Resistência do Ar", BranchId = 10 },
                new Topics { Id = 95, Name = "Dinâmica do movimento circular", BranchId = 10 },
                new Topics { Id = 96, Name = "Trabalho e Energia", BranchId = 10 },
                new Topics { Id = 97, Name = "Impulso e Quantidade de Movimento", BranchId = 10 },
                new Topics { Id = 98, Name = "Colisões", BranchId = 10 },
                new Topics { Id = 99, Name = "Equilíbrio de Corpo Extenso", BranchId = 10 },
                new Topics { Id = 100, Name = "Leis de Kepler", BranchId = 10 },
                new Topics { Id = 101, Name = "Gravitação", BranchId = 10 },
                new Topics { Id = 102, Name = "Hidrostática", BranchId = 10 },
                new Topics { Id = 103, Name = "Hidrodinâmica", BranchId = 10 },
                new Topics { Id = 104, Name = "Análise Dimensional", BranchId = 10 },

                // TERMODINÂMICA
                new Topics { Id = 105, Name = "Temperatura", BranchId = 11 },
                new Topics { Id = 106, Name = "Propagação do Calor", BranchId = 11 },
                new Topics { Id = 107, Name = "Dilatometria", BranchId = 11 },
                new Topics { Id = 108, Name = "Calorimetria", BranchId = 11 },
                new Topics { Id = 109, Name = "Calorimetria (Aprofundamento)", BranchId = 11 },
                new Topics { Id = 110, Name = "Gases", BranchId = 11 },
                new Topics { Id = 111, Name = "Lei Zero da Termodinâmica", BranchId = 11 },
                new Topics { Id = 112, Name = "1ª Lei da Termodinâmica", BranchId = 11 },
                new Topics { Id = 113, Name = "2ª Lei da Termodinâmica", BranchId = 11 },

                // ÓPTICA
                new Topics { Id = 114, Name = "Fundamentos da Óptica Geométrica", BranchId = 12 },
                new Topics { Id = 115, Name = "Espelhos Planos", BranchId = 12 },
                new Topics { Id = 116, Name = "Espelhos Esféricos", BranchId = 12 },
                new Topics { Id = 117, Name = "Refração", BranchId = 12 },
                new Topics { Id = 118, Name = "Lentes", BranchId = 12 },
                new Topics { Id = 119, Name = "Instrumentos Ópticos e Olho Humano", BranchId = 12 },

                // ONDAS
                new Topics { Id = 120, Name = "Movimento Harmônico Simples (MHS)", BranchId = 13 },
                new Topics { Id = 121, Name = "Introdução à Ondulatória", BranchId = 13 },
                new Topics { Id = 122, Name = "Fenômenos Ondulatórios", BranchId = 13 },
                new Topics { Id = 123, Name = "Ondas Estacionárias", BranchId = 13 },
                new Topics { Id = 124, Name = "Acústica", BranchId = 13 },

                // ELETRICIDADE
                new Topics { Id = 125, Name = "Carga Elétrica e Processos de Eletrização", BranchId = 14 },
                new Topics { Id = 126, Name = "Força Elétrica", BranchId = 14 },
                new Topics { Id = 127, Name = "Campo Elétrico", BranchId = 14 },
                new Topics { Id = 128, Name = "Potencial Elétrico", BranchId = 14 },
                new Topics { Id = 129, Name = "Condutores", BranchId = 14 },
                new Topics { Id = 130, Name = "Corrente Elétrica", BranchId = 14 },
                new Topics { Id = 131, Name = "Lei de Ohm e Resistores", BranchId = 14 },
                new Topics { Id = 132, Name = "Associação de Resistores", BranchId = 14 },
                new Topics { Id = 133, Name = "Instrumentos de Medida Elétrica", BranchId = 14 },
                new Topics { Id = 134, Name = "Geradores e Receptores", BranchId = 14 },
                new Topics { Id = 135, Name = "Capacitores", BranchId = 14 },
                new Topics { Id = 136, Name = "Campo Magnético", BranchId = 14 },
                new Topics { Id = 137, Name = "Força Magnética", BranchId = 14 },
                new Topics { Id = 138, Name = "Fontes de Campo Magnético", BranchId = 14 },
                new Topics { Id = 139, Name = "Indução Eletromagnética", BranchId = 14 },

                // FÍSICA MODERNA
                new Topics { Id = 140, Name = "Física Quântica", BranchId = 15 },
                new Topics { Id = 141, Name = "Efeito Fotoelétrico e Comportamento Dual", BranchId = 15 },
                new Topics { Id = 142, Name = "Relatividade Especial", BranchId = 15 },
                new Topics { Id = 143, Name = "Física Nuclear", BranchId = 15 }
            );


            //PlanTimeSpans


            modelBuilder.Entity<PlanTimeSpans>().HasData(
                new PlanTimeSpans { Id = 1, Name = "Extensivo", Duration = TimeSpan.FromDays(365) },
                new PlanTimeSpans { Id = 2, Name = "Intensivo", Duration = TimeSpan.FromDays(180) },
                new PlanTimeSpans { Id = 3, Name = "Mensal", Duration = TimeSpan.FromDays(30) },
                new PlanTimeSpans { Id = 4, Name = "Vitalício", Duration = TimeSpan.MaxValue }
            );
        }
    }
}