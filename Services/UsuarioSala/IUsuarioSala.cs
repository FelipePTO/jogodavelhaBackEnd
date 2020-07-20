using System.Collections.Generic;

namespace UsuarioSenhaFuncoes{
    public interface IUsuarioSenha
    {
        bool usuarioConectou(string usuarioNome);
        bool usuarioDesconectou(string usuarioNome);
        List<string> ListarUsuariosConectados();
        
    }
}