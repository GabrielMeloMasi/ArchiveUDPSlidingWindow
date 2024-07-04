using System; // Importa funcionalidades básicas do .NET
using System.Net; // Importa funcionalidades de rede
using System.Net.Sockets; // Importa funcionalidades de sockets
using System.Collections.Generic; // Importa funcionalidades de coleções genéricas
using System.IO; // Importa funcionalidades de manipulação de arquivos
using System.Text; // Importa funcionalidades de manipulação de strings

class FileDownloader // Define a classe para download de arquivos
{
    public static void DownloadFile(UdpClient client, IPEndPoint serverEP, string fileName) // Método para baixar um arquivo
    {
        client.Send(Encoding.UTF8.GetBytes($"DOWNLOAD|{fileName}"), Encoding.UTF8.GetByteCount($"DOWNLOAD|{fileName}"), serverEP); // Envia o comando de download com o nome do arquivo

        List<byte> fileData = new List<byte>(); // Lista para armazenar os dados do arquivo
        byte[] receivedData; // Array de bytes para armazenar dados recebidos
        int expectedPacket = 0; // Número do pacote esperado
        int windowSize = 5; // Tamanho da janela deslizante
        Dictionary<int, byte[]> window = new Dictionary<int, byte[]>(); // Dicionário para armazenar pacotes na janela

        while (true) // Loop para receber todos os pacotes
        {
            receivedData = client.Receive(ref serverEP); // Recebe um pacote do servidor
            int packetNumber = BitConverter.ToInt32(receivedData, 0); // Converte o número do pacote em inteiro
            byte[] packetData = new byte[receivedData.Length - 4]; // Cria um array de bytes para os dados do pacote
            Array.Copy(receivedData, 4, packetData, 0, packetData.Length); // Copia os dados do pacote para o array

            if (!window.ContainsKey(packetNumber)) // Se o pacote ainda não foi recebido
            {
                window[packetNumber] = packetData; // Armazena o pacote na janela
            }

            while (window.ContainsKey(expectedPacket)) // Processa pacotes na ordem esperada
            {
                fileData.AddRange(window[expectedPacket]); // Adiciona os dados do pacote à lista
                window.Remove(expectedPacket); // Remove o pacote da janela
                expectedPacket++; // Incrementa o número do pacote esperado
            }

            client.Send(BitConverter.GetBytes(expectedPacket), 4, serverEP); // Envia a confirmação para o servidor

            if (packetData.Length < 1024) // Se o pacote recebido é menor que 1024 bytes (último pacote)
                break; // Encerra o loop
        }

        File.WriteAllBytes(fileName, fileData.ToArray()); // Escreve os dados do arquivo no disco
        Console.WriteLine($"Arquivo {fileName} baixado."); // Exibe mensagem de sucesso
    }
}
