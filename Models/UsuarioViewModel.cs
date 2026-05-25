using System.ComponentModel.DataAnnotations;

namespace ProjectBarber.Models
{
    public class UsuarioViewModel
    {
      
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }            
    }
}
