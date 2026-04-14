using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TiendaVentas.Web.Models;

namespace TiendaVentas.Web.Services
{
    public class PedidoPdfService
    {
        private readonly IWebHostEnvironment _env;

        public PedidoPdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GenerarPdf(int idPedido, PedidoCheckoutViewModel model)
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "logo-mc-nails.jpg");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Row(row =>
                    {
                        row.ConstantItem(80).Height(80).Image(logoPath);

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MC Nails").FontSize(20).Bold();
                            col.Item().Text("Solicitud Formal de Pedido").FontSize(14);
                            col.Item().Text($"Pedido No.: {idPedido}").FontSize(11);
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(11);
                        });
                    });

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text("Datos del cliente").Bold().FontSize(14);

                        col.Item().Text($"Nombre: {model.Nombre_Cliente}");
                        col.Item().Text($"Teléfono: {model.Telefono}");
                        col.Item().Text($"Correo: {model.Correo_Cliente ?? "-"}");
                        col.Item().Text($"Dirección: {model.Direccion ?? "-"}");
                        col.Item().Text($"Observaciones: {model.Observaciones ?? "-"}");

                        col.Item().PaddingTop(10).Text("Detalle del pedido").Bold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Producto").Bold();
                                header.Cell().Element(CellStyle).AlignCenter().Text("Cant.").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Precio").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Subtotal").Bold();
                            });

                            foreach (var item in model.Items)
                            {
                                table.Cell().Element(CellStyle).Text(item.Nombre);
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());
                                table.Cell().Element(CellStyle).AlignRight().Text($"Q {item.Precio:N2}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"Q {item.Subtotal:N2}");
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(6).PaddingHorizontal(4);
                            }
                        });

                        col.Item().AlignRight().PaddingTop(10).Text($"TOTAL: Q {model.Total:N2}").Bold().FontSize(16);
                    });

                    page.Footer().AlignCenter().Text("MC Nails - Pedido generado desde el sistema").FontSize(10);
                });
            });

            return document.GeneratePdf();
        }
    }
}