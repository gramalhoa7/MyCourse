using MyCourse.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyCourse.Services;

public static class ListPdf
{
    private const int MaxImageBytes = 5 * 1024 * 1024;

    // ---------- Imagens ----------

    // Baixa todas as imagens da lista (enunciados e alternativas), até 8 por vez
    public static async Task<Dictionary<string, byte[]>> DownloadImagesAsync(HttpClient http, List<ListsQuestions> items)
    {
        var urls = items
            .SelectMany(lq => lq.Question.Alternatives.Select(a => a.ImageUrl).Append(lq.Question.ImageUrl))
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct()
            .ToList();

        var result = new Dictionary<string, byte[]>();
        using var limit = new SemaphoreSlim(8);

        var tasks = urls.Select(async url =>
        {
            await limit.WaitAsync();
            try
            {
                var bytes = await TryDownloadAsync(http, url);
                if (bytes != null)
                {
                    lock (result) result[url] = bytes;
                }
            }
            finally
            {
                limit.Release();
            }
        });

        await Task.WhenAll(tasks);
        return result;
    }

    private static async Task<byte[]?> TryDownloadAsync(HttpClient http, string url)
    {
        // Só endereços completos http/https. Caminhos relativos ainda não são suportados.
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return null;

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var response = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            if (!response.IsSuccessStatusCode) return null;
            if (response.Content.Headers.ContentLength > MaxImageBytes) return null;

            var bytes = await response.Content.ReadAsByteArrayAsync(cts.Token);
            return bytes.Length <= MaxImageBytes && LooksLikeImage(bytes) ? bytes : null;
        }
        catch
        {
            return null;
        }
    }

    // O PDF só aceita PNG e JPEG; qualquer outra coisa vira "imagem indisponível"
    private static bool LooksLikeImage(byte[] b) =>
        b.Length > 8 &&
        ((b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) ||   // PNG
         (b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF));                    // JPEG

    // ---------- Capa ----------

    // Procura a arte da capa na pasta Assets (ao lado do programa):
    //   cover-matematica.png / cover-fisica.png  (uma por matéria, opcional)
    //   cover.png                                 (uma para todas)
    // Aceita .png, .jpg e .jpeg. Se não achar nada, a capa usa um bloco colorido no lugar.
    public static byte[]? LoadCoverArt(Matters matter)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "Assets");
        var slug = matter switch
        {
            Matters.Matemática => "matematica",
            Matters.Física => "fisica",
            _ => "default"
        };

        foreach (var name in new[] { $"cover-{slug}", "cover" })
        {
            foreach (var extension in new[] { ".png", ".jpg", ".jpeg" })
            {
                var path = Path.Combine(folder, name + extension);
                if (!File.Exists(path)) continue;

                var bytes = File.ReadAllBytes(path);
                if (LooksLikeImage(bytes)) return bytes;
            }
        }

        return null;
    }

    // Monta o endereço da resolução a partir do modelo (ex: https://meusite.com/resolucao/{id})
    private static string? BuildResolutionUrl(string? template, int questionId)
    {
        if (string.IsNullOrWhiteSpace(template)) return null;

        var url = template.Replace("{id}", questionId.ToString());
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return null;

        return url;
    }

    // ---------- PDF ----------

    // Estrutura: capa, páginas de questões (no máximo 2 por página, lado a lado, divididas por uma linha vertical)
    // e gabarito no final. Se sideBySide for false, usa 1 questão por página em largura total (plano B).
    public static byte[] Generate(Lists list, List<ListsQuestions> items, Dictionary<string, byte[]> images,
        byte[]? coverArt, string? resolutionUrlTemplate, bool sideBySide)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        void ShowImage(IContainer container, string url, float maxWidth)
        {
            if (images.TryGetValue(url, out var bytes))
                container.MaxWidth(maxWidth).Image(bytes).FitWidth();
            else
                container.Text("[imagem indisponível]").FontColor(Colors.Grey.Medium).Italic();
        }

        void RenderQuestion(IContainer container, int number, Questions question, float imageWidth, float alternativeImageWidth)
        {
            container.Column(block =>
            {
                block.Spacing(6);
                block.Item().Text($"Questão {number}").SemiBold().FontSize(12);

                if (!string.IsNullOrWhiteSpace(question.Content))
                    block.Item().Text(question.Content);

                if (!string.IsNullOrWhiteSpace(question.ImageUrl))
                    ShowImage(block.Item(), question.ImageUrl, imageWidth);

                foreach (var alt in question.Alternatives.OrderBy(a => a.Letter))
                {
                    block.Item().Row(row =>
                    {
                        row.ConstantItem(22).Text($"{alt.Letter})").SemiBold();
                        row.RelativeItem().Column(c =>
                        {
                            var hasText = !string.IsNullOrWhiteSpace(alt.Content);
                            var hasImage = !string.IsNullOrWhiteSpace(alt.ImageUrl);

                            if (hasText) c.Item().Text(alt.Content);
                            if (hasImage) ShowImage(c.Item(), alt.ImageUrl, alternativeImageWidth);
                            if (!hasText && !hasImage) c.Item().Text("—");
                        });
                    });
                }
            });
        }

        // Questões numeradas na ordem da lista, em grupos de 2 por página (ou 1, no plano B)
        var pages = items
            .Select((lq, index) => (Question: lq.Question, Number: index + 1))
            .Chunk(sideBySide ? 2 : 1)
            .ToList();

        // Gabarito: letra correta de cada questão + link para a resolução
        var key = items
            .Select((lq, index) => new
            {
                Number = index + 1,
                Answer = lq.Question.Alternatives.Any(a => a.IsCorrect)
                    ? string.Join(", ", lq.Question.Alternatives.Where(a => a.IsCorrect).OrderBy(a => a.Letter).Select(a => a.Letter))
                    : "—",
                Url = BuildResolutionUrl(resolutionUrlTemplate, lq.Question.Id)
            })
            .ToList();

        return Document.Create(document =>
        {
            // 1) Capa: arte, nome da matéria e o Id da lista
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);

                page.Content().Column(col =>
                {
                    col.Item().Height(560).Element(art =>
                    {
                        if (coverArt != null)
                            art.Image(coverArt).FitArea();
                        else
                            art.Background(Colors.Blue.Darken3).AlignCenter().AlignMiddle()
                               .Text("Arte da capa").FontColor(Colors.White).FontSize(24);
                    });

                    col.Item().Padding(40).Column(info =>
                    {
                        info.Spacing(8);
                        info.Item().Text(list.Matter.ToString()).FontSize(40).Bold();
                        info.Item().Text($"Lista nº {list.Id}").FontSize(20).FontColor(Colors.Grey.Darken2);
                    });
                });
            });

            // 2) Questões: no máximo 2 por página, lado a lado, com uma linha vertical no meio
            foreach (var chunk in pages)
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().PaddingBottom(8).Row(row =>
                    {
                        row.RelativeItem().Text(list.Title).SemiBold();
                        row.AutoItem().Text($"{list.Matter} · Lista nº {list.Id}").FontColor(Colors.Grey.Darken1);
                    });

                    if (sideBySide)
                    {
                        page.Content().Row(row =>
                        {
                            row.RelativeItem().PaddingRight(10).Element(left =>
                                RenderQuestion(left, chunk[0].Number, chunk[0].Question, 250, 150));

                            row.AutoItem().LineVertical(1).LineColor(Colors.Grey.Lighten1);

                            row.RelativeItem().PaddingLeft(10).Element(right =>
                            {
                                // Na última página, se a lista tiver número ímpar de questões, a coluna da direita fica vazia
                                if (chunk.Length > 1)
                                    RenderQuestion(right, chunk[1].Number, chunk[1].Question, 250, 150);
                            });
                        });
                    }
                    else
                    {
                        page.Content().Element(content =>
                            RenderQuestion(content, chunk[0].Number, chunk[0].Question, 535, 300));
                    }

                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(9));
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            }

            // 3) Gabarito: cada resposta é um link para a resolução da questão
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().PaddingBottom(8).Column(header =>
                {
                    header.Item().Text("Gabarito").FontSize(18).SemiBold();
                    header.Item().Text("Clique em uma resposta para abrir a resolução da questão.")
                        .FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < 5; i++) columns.RelativeColumn();
                    });

                    foreach (var entry in key)
                    {
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6).Element(cell =>
                        {
                            if (entry.Url != null)
                            {
                                cell.Hyperlink(entry.Url).Text(t =>
                                {
                                    t.Span($"{entry.Number}. ").SemiBold();
                                    t.Span(entry.Answer).Underline().FontColor(Colors.Blue.Medium);
                                });
                            }
                            else
                            {
                                cell.Text(t =>
                                {
                                    t.Span($"{entry.Number}. ").SemiBold();
                                    t.Span(entry.Answer);
                                });
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.DefaultTextStyle(x => x.FontSize(9));
                    t.Span("Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
