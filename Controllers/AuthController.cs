using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjectBarber.Data;
using ProjectBarber.Models;
using System.Security.Claims;

namespace ProjectBarber.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioData _usuarioData;      

        public AuthController(UsuarioData usuarioData) 
        {
            _usuarioData = usuarioData;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        //UsuarioViewModel é uma classe que serve para receber os dados do formulário de login, ela deve conter as propriedades Email e Senha   
        public async Task<IActionResult> RealizarLogin([FromBody] UsuarioLoginViewModel usuarioLoginViewModel)
        {
            //verificaa se o usuario digitou corretamente o email e a senha
            if (string.IsNullOrEmpty(usuarioLoginViewModel.Email) || string.IsNullOrEmpty(usuarioLoginViewModel.Senha))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }


            var usuarioDb = _usuarioData.BuscarUsuarioPorEmail(usuarioLoginViewModel.Email);

            if (usuarioDb == null)
            {
                //se o email não for encontrado, retorna uma resposta de erro indicando que as credenciais são inválidas
                return Unauthorized(new { mensagem = "E-mail inválido." });
            }

            //verifica se a senha123 QUANDO CRIPTOGRAFADA É IGUAL A usuarioDb.Senha que ja esta criptografada, pois ela vem do banco
            if (!BCrypt.Net.BCrypt.Verify(usuarioLoginViewModel.Senha, usuarioDb.Senha))
            {
                return Unauthorized(new { mensagem = "Senha inválida." });
            }

            //monta o cracha do usuario para identificar ele e realizar a autenticação, o cracha é composto por uma lista de claims,
            //que são informações sobre o usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDb.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDb.Nome),
                new Claim(ClaimTypes.Email, usuarioDb.Email),
                new Claim(ClaimTypes.Role, usuarioDb.Role)
            };

            // cria a identidade do usuário com base nos claims e no esquema de autenticação de cookies
            var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var propriedadesDeAutenticacao = new AuthenticationProperties { IsPersistent = true }; // Mantém logado (lembrar-me)

            // Efetivamente cria o Cookie de segurança no navegador
            // O método SignInAsync é responsável por criar o cookie de autenticação e associá-lo à identidade do usuário. Ele recebe o esquema de autenticação, a identidade do usuário e as propriedades de autenticação.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidade),
                propriedadesDeAutenticacao);

            // Devolve sucesso para o JavaScript, que fará o redirecionamento
            return Ok(new { sucesso = true });
        }


        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        //rota quer o fetch utiliza para enviar os dados do formulário de cadastro
        //o método é POST e recebe um objeto do tipo Usuario no corpo da requisição
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            if (usuario == null)
            { 
                return BadRequest("Dados do usuário são obrigatórios.");    
            }

            
            _usuarioData.CadastrarUsuario(usuario);


           return View("Cadastrar",usuario);
        }

    }
}
