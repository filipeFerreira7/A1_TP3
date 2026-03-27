namespace A1_order_system.Entities
{
    public class Endereco
    {
        public long Id { get; set; }

        public string Logradouro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;

        public string Estado {  get; set; } = string.Empty;

        public long UsuarioId {get; set;}
        public Usuario Usuario { get; set; } = null!;

    }
}
