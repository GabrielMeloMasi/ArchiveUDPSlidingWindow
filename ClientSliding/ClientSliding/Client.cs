using System; // Importa as funcionalidades básicas do .NET
using System.Net; // Importa funcionalidades de rede
using System.Net.Sockets; // Importa funcionalidades de sockets
using System.Text; // Importa funcionalidades para manipulação de strings
using System.IO; // Importa funcionalidades de manipulação de arquivos

class Client // Define a classe principal para o cliente
{
    static void Main(string[] args) // Método principal, ponto de entrada do programa
    {
        UdpClient client = new UdpClient(); // Cria uma instância do cliente UDP
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // Define o endereço do servidor
        Console.WriteLine("Cliente iniciado..."); // Exibe mensagem indicando que o cliente foi iniciado

        while (true) // Loop infinito para manter o cliente ativo
        {
            Console.WriteLine("Escolha uma opção: UPLOAD, LIST, DOWNLOAD"); // Solicita uma opção ao usuário
            string option = Console.ReadLine(); // Lê a opção do usuário

            if (option == "UPLOAD") // Se a opção for UPLOAD:
            {
                Console.Write("Digite o caminho do arquivo: "); // Solicita o caminho do arquivo
                string filePath = Console.ReadLine(); // Lê o caminho do arquivo
                FileUploader.UploadFile(client, serverEP, filePath); // Chama o método de upload do arquivo
            }
            else if (option == "LIST") // Se a opção for LIST:
            {
                client.Send(Encoding.UTF8.GetBytes("LIST"), "LIST".Length, serverEP); // Envia o comando LIST para o servidor
                byte[] fileListData = client.Receive(ref serverEP); // Recebe a lista de arquivos do servidor
                string fileList = Encoding.UTF8.GetString(fileListData); // Converte os dados recebidos em string
                Console.WriteLine($"Arquivos no servidor: {fileList}"); // Exibe a lista de arquivos
            }
            else if (option == "DOWNLOAD") // Se a opção for DOWNLOAD:
            {
                Console.Write("Digite o nome do arquivo para download: "); // Solicita o nome do arquivo
                string fileName = Console.ReadLine(); // Lê o nome do arquivo
                FileDownloader.DownloadFile(client, serverEP, fileName); // Chama o método de download do arquivo
            }
        }
    }
}
