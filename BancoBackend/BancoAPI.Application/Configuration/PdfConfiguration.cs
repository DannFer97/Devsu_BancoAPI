namespace BancoAPI.Application.Configuration
{
    /// <summary>
    /// Configuración  para generación de PDFs
    /// </summary>
    public class PdfConfiguration
    {

        public PdfColors Colors { get; set; } = new();
        public PdfFonts Fonts { get; set; } = new();
        public CompanyInfo Company { get; set; } = new();
    }

    public class PdfColors
    {
        public string Primary { get; set; } = "#3498db";
        public string Secondary { get; set; } = "#2c3e50";
        public string Success { get; set; } = "#27ae60";
        public string Danger { get; set; } = "#e74c3c";
        public string Warning { get; set; } = "#f39c12";
        public string Light { get; set; } = "#ecf0f1";
        public string Dark { get; set; } = "#34495e";
    }

    public class PdfFonts
    {
        public int HeaderSize { get; set; } = 24;
        public int SubHeaderSize { get; set; } = 16;
        public int NormalSize { get; set; } = 11;
        public int SmallSize { get; set; } = 9;
        public string FontFamily { get; set; } = "Arial";
    }

    public class CompanyInfo
    {
        public string Name { get; set; } = "Banco Devsu";
        public string Address { get; set; } = "Quito, Ecuador";
        public string Phone { get; set; } = "+593 2 1234567";
        public string Email { get; set; } = "info@bancodevsu.com";
        public string Website { get; set; } = "www.bancodevsu.com";
    }
}
