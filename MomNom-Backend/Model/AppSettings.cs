namespace MomNom_Backend.Model
{

    public class Edamam
    {
        public string AppId { get; private set; }
        public string AppKey { get; private set; }

        public Edamam(IConfiguration configuration)
        {
            AppId = configuration.GetValue<string>("AppId") ?? "";
            AppKey = configuration.GetValue<string>("AppKey") ?? "";
        }
    }

    public class Secret
    {
        public Edamam Edamam { get; private set; }

        public Secret(IConfiguration configuration)
        {
            Edamam = new Edamam(configuration.GetSection("Edamam"));
        }
    }

    public class Email
    {
        public string SmtpServer { get; private set; }
        public int SmtpPort { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Email(IConfiguration configuration)
        {
            SmtpServer = configuration.GetValue<string>("SmtpServer") ?? "";
            SmtpPort = configuration.GetValue<int>("SmtpPort");
            Username = configuration.GetValue<string>("Username") ?? "";
            Password = configuration.GetValue<string>("Password") ?? "";
        }
    }

        public static class AppSettings
    {
        public static Secret Secrets { get; private set; }
        public static string FoodDetectionEndpoint { get; private set; }
        public static Email Email { get; private set; }
        public static string frontEndUri { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            Secrets = new Secret(configuration.GetSection("Secrets"));
            FoodDetectionEndpoint = configuration.GetValue<string>("FoodDetectionEndpoint") ?? "";
            Email = new Email(configuration.GetSection("Email"));
            frontEndUri = configuration.GetValue<string>("FrontEndUri") ?? "https://momnom.jasson.my.id";
            // Or bind a section: configuration.GetSection("MySection").Bind(MyStaticObject);
        }
    }
}
