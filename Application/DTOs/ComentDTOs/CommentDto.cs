namespace Application.DTOs.ComentDTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } // <- corregido
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        
        public string ProductName { get; set; } // Nombre del producto
        public string UserName { get; set; }    // Nombre del usuario
        
    }
}
