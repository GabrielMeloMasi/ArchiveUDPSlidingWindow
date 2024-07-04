using System; // Importa funcionalidades básicas do .NET
using System.Net; // Importa funcionalidades de rede
using System.Net.Sockets; // Importa funcionalidades de sockets
using System.IO; // Importa funcionalidades de manipulação de arquivos
using System.Text; // Importa funcionalidades de manipulação de strings
using System.Collections.Generic; // Importa funcionalidades de coleções genéricas

class FileUploader // Define a classe para upload de arquivos
{
    public static void UploadFile(UdpClient client, IPEndPoint serverEP, string filePath) // Método para enviar um arquivo
    {
        string fileName = Path.GetFileName(filePath); // Obtém o nome do arquivo a partir do caminho
        client.Send(Encoding.UTF8.GetBytes($"UPLOAD|{fileName}"), Encoding.UTF8.GetByteCount($"UPLOAD|{fileName}"), serverEP); // Envia o comando de upload com o nome do arquivo

        byte[] fileData = File.ReadAllBytes(filePath); // Lê os dados do arquivo em um array de bytes
        int totalPackets = (fileData.Length / 1024) + 1; // Calcula o número total de pacotes
        int windowSize = 5; // Tamanho da janela deslizante
        int baseIndex = 0; // Índice base da janela
        int nextSeqNum = 0; // Próximo número de sequência
        Dictionary<int, byte[]> window = new Dictionary<int, byte[]>(); // Dicionário para armazenar pacotes na janela

        while (baseIndex < totalPackets) // Loop até enviar todos os pacotes
        {
            while (nextSeqNum < baseIndex + windowSize && nextSeqNum < totalPackets) // Envia pacotes dentro da janela
            {
                int start = nextSeqNum * 1024; // Calcula o início dos dados do pacote
                int length = Math.Min(1024, fileData.Length - start); // Calcula o comprimento do pacote
                byte[] packetData = new byte[4 + length]; // Cria um array de bytes para o pacote
                Array.Copy(BitConverter.GetBytes(nextSeqNum), packetData, 4); // Copia o número de sequência para o pacote
                Array.Copy(fileData, start, packetData, 4, length); // Copia os dados do arquivo para o pacote
                client.Send(packetData, packetData.Length, serverEP); // Envia o pacote para o servidor
                window[nextSeqNum] = packetData; // Armazena o pacote na janela
                nextSeqNum++; // Incrementa o número de sequência
            }

            byte[] ackData = client.Receive(ref serverEP); // Recebe a confirmação do servidor
            int ack = BitConverter.ToInt32(ackData, 0); // Converte a confirmação em inteiro

            if (ack >= baseIndex) // Se a confirmação for válida
            {
                baseIndex = ack + 1; // Atualiza o índice base
                for (int i = baseIndex; i < baseIndex + windowSize; i++) // Remove pacotes confirmados da janela
                {
                    window.Remove(i);
                }
            }
        }

        Console.WriteLine($"Arquivo {fileName} enviado."); // Exibe mensagem de sucesso
    }
}
