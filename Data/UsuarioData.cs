using MySqlConnector;
using ProjectBarber.Models;


namespace ProjectBarber.Data
{
    public class UsuarioData
    {
        private readonly string _connectionString;

        //método construtor serve para inicializar um objeto no qual vai servir 
        //para receber a string de conexão do banco de dados      
        public UsuarioData(string connectionString)
        { 
            _connectionString = connectionString;           
        }

        public void CadastrarUsuario(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            { 
                conexao.Open();

                string query = "INSERT INTO Usuarios (Nome, Email, Telefone, Senha) VALUES (@Nome, @Email, @Telefone, @Senha)";

                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Nome", usuario.Nome);
                    comando.Parameters.AddWithValue("@Email", usuario.Email);
                    comando.Parameters.AddWithValue("@Telefone", usuario.Telefone);

                    string senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

                    comando.Parameters.AddWithValue("@Senha", senhaCriptografada);

                    comando.ExecuteNonQuery();              
                }

            
            }


        
        }


        public Usuario BuscarUsuarioPorEmail(string email)
        {
            using (var conexao = new MySqlConnection(_connectionString))
            {
                conexao.Open();
                string query = "SELECT Id, Nome, Email, Senha, Role FROM usuarios WHERE Email = @Email";

                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);

                    using (var leitor = comando.ExecuteReader())
                    {
                        //se encontrado o email, o reader irá ler a linha retornada pela consulta SQL
                        if (leitor.Read())
                        {
                            //se o usuário for encontrado, cria um objeto Usuario e retorna
                            return new Usuario
                            {
                                Id = Convert.ToInt32(leitor["Id"]),
                                Nome = leitor["Nome"].ToString(),
                                Email = leitor["Email"].ToString(),
                                Senha = leitor["Senha"].ToString(),  //hash da senha criptografada
                                Role = leitor["Role"].ToString() //adiciona a role do usuário
                            };
                           

                        }
                    }               

                }           

            }

            return null; //retorna null se o usuário não for encontrado 
        }
    }
}
