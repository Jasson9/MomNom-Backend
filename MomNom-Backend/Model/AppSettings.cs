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
    
    public static class AppSettings
    {
        public static Secret Secrets { get; private set; }
        public static string FoodDetectionEndpoint { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            Secrets = new Secret(configuration.GetSection("Secrets"));
            FoodDetectionEndpoint = configuration.GetValue<string>("FoodDetectionEndpoint") ?? "";
            // Or bind a section: configuration.GetSection("MySection").Bind(MyStaticObject);
        }
    }
}
