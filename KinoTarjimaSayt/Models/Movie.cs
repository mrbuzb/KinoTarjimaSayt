namespace KinoTarjimaMVC.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }          // Kino nomi
        public string VideoPath { get; set; }      // Video fayl manzili
        public string SubtitlePath { get; set; }   // Subtitr fayl manzili
    }
}
