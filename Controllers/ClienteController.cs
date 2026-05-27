using Microsoft.AspNetCore.Mvc;
using ProjectBarber.Data;
using ProjectBarber.Models;

namespace ProjectBarber.Controllers
{
    public class ClienteController : Controller
    {
        private readonly UsuarioData _usuarioData;
        

        public ClienteController(UsuarioData usuarioData)
        {
            _usuarioData = usuarioData;
          
        }

        [HttpGet]
        public IActionResult Exibir()
        {
            
            try
            {
                List<Usuario> usuarios = new List<Usuario>();

                foreach (var usuario in _usuarioData.ListarUsuarios())
                {
                          
                    if (usuario.Role == "comum")
                    {
                        usuarios.Add(usuario);
                       
                    }
                }

                if (usuarios == null || usuarios.Count == 0)
                {
                    return NotFound("Nenhum cliente encontrado.");
                }

                return View(usuarios);


            }
            catch (Exception ex) {
                return BadRequest("Erro: "+ex.Message+" : "+ex.StackTrace);
            }

            

        }
    }
}
