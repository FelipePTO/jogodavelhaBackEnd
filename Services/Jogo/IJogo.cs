using System.Collections.Generic;
using configSistema;
using SalaParametros;

namespace JogoFuncoes{
    public interface IJogo
    {
        RetornoModel loginUsuario(string usuario);
        RetornoModel CriarSala(string nomesala, string usuario);
        RetornoModel SairSala(string nomesala, string usuario);
        List<SalaModel> listaSalasAbertas();
        RetornoModel EntrarSala(string nomesala, string usuario);
    }
}