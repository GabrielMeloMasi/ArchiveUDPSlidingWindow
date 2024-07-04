using System; // Importa funcionalidades básicas do .NET
using System.Net; // Importa funcionalidades de rede
using System.Net.Sockets; // Importa funcionalidades de sockets
using System.Text; // Importa funcionalidades de manipulação de strings
using System.IO; // Importa funcionalidades de manipulação de arquivos
using System.Collections.Generic; // Importa funcionalidades de coleções genéricas

class Server // Define a classe principal para o servidor
{
    static void Main(string[] args) // Método principal, ponto de entrada do programa
    {
        UdpClient server = new UdpClient(11000); // Cria uma instância do servidor UDP na porta 11000
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0); // Define um endpoint remoto para aceitar qualquer IP e porta
        Console.WriteLine("Servidor iniciado..."); // Exibe mensagem indicando que o servidor foi iniciado
        Directory.CreateDirectory("uploads"); // Cria o diretório "uploads" se não existir

        while (true) // Loop infinito para manter o servidor ativo
        {
            byte[] receivedData = server.Receive(ref remoteEP); // Recebe dados de um cliente
            string message = Encoding.UTF8.GetString(receivedData); // Converte os dados recebidos em string
            Console.WriteLine($"Mensagem recebida: {message}"); // Exibe a mensagem recebida

            if (message.StartsWith("UPLOAD")) // Se a mensagem começar com "UPLOAD"
            {
                string fileName = message.Split('|')[1]; // Obtém o nome do arquivo a partir da mensagem
                FileReceiver.ReceiveFile(server, remoteEP, fileName); // Chama o método para receber o arquivo
            }
            else if (message == "LIST") // Se a mensagem for "LIST"
            {
                string fileList = string.Join(",", Directory.GetFiles("uploads")); // Obtém a lista de arquivos no diretório "uploads"
                byte[] fileListData = Encoding.UTF8.GetBytes(fileList); // Converte a lista de arquivos em bytes
                server.Send(fileListData, fileListData.Length, remoteEP); // Envia a lista de arquivos para o cliente
            }
            else if (message.StartsWith("DOWNLOAD")) // Se a mensagem começar com "DOWNLOAD"
            {
                string fileName = message.Split('|')[1]; // Obtém o nome do arquivo a partir da mensagem
                FileSender.SendFile(server, remoteEP, fileName); // Chama o método para enviar o arquivo
            }
        }
    }
}
