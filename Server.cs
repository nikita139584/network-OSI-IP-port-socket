using System.Net; // основний простір імен для роботи з мережевими адресами та протоколами
using System.Net.Sockets; // простір імен для роботи з сокетами
using System.Text; // простір імен для роботи з кодуваннями

class Server
{
    private const int MESSAGE_LENGTH = 512; // задає розмір буфера для отримання даних
    private const int SERVER_PORT = 27015; // вказує порт, на якому сервер прослуховуватиме підключення
    private const int PAUSE = 0; // задає паузу в мілісекундах для краси та зручності виведення повідомлень (можна сміливо прибрати)

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "SERVER";

        try
        {
            var ipAddress = IPAddress.Any; // отримує будь-яку доступну IP-адресу для прослуховування (означає, що сервер слухатиме на всіх інтерфейсах, наприклад, Wi-Fi, Ethernet
            var localEndPoint = new IPEndPoint(ipAddress, SERVER_PORT); // створює кінцеву точку (адресу та порт), до якої сервер буде прив’язаний

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // створює сокет для використання TCP-з’єднання (потоковий сокет)
            listener.Bind(localEndPoint); // прив’язує сокет до вказаної адреси та порту

            listener.Listen(10); // починає прослуховування вхідних з’єднань, встановлюючи максимальну кількість з’єднань, що очікують (10), тобто сервер може мати до 10 клієнтів (з’єднань) у черзі на підключення

            var clientSocket = listener.Accept(); // очікує підключення клієнта та приймає його, повертаючи сокет для спілкування з клієнтом. є AcceptAsync(), щоб не блокувати потік

            while (true)
            {
                var buffer = new byte[MESSAGE_LENGTH];
                int bytesReceived = clientSocket.Receive(buffer);

                if (bytesReceived == 0)
                {
                    Console.WriteLine("З’єднання закривається...");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Клієнт надіслав: {message}");

                if (int.TryParse(message, out int number))
                {
                    string response;

                    switch (number)
                    {
                        case 1:
                            response = "Привіт";
                            break;
                        case 2:
                            response = "Як справи?";
                            break;
                        case 3:
                            response = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                            break;
                        case 4:
                            response = "Сервер тут!";
                            break;
                        case 5:
                            response = "Окей";
                            break;
                        case 6:
                            response = "Бувай!";
                            break;
                        default:
                            response = "Невідома команда";
                            break;
                    }

                    Console.WriteLine(response);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response); 
                    clientSocket.Send(responseBytes);

                }
                else
                {
                    string response = "Це не число!";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(responseBytes);
                }
            }

            Console.WriteLine("Процес сервера завершує свою роботу!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
    }
}
