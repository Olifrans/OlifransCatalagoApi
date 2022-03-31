using System.Text.Json.Serialization;

namespace Catalago.Api.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        //[JsonIgnore] //Vai ignorar a propriedade logica de produto na deserealização
        public ICollection<Produto>? Produtos { get; set; }
    }
}