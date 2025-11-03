namespace MomNom_Backend.Model.Response
{

    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class EdamamNutritionDataResponse
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("dietLabels")]
        public List<string> DietLabels { get; set; }

        [JsonPropertyName("healthLabels")]
        public List<string> HealthLabels { get; set; }

        [JsonPropertyName("cautions")]
        public List<string> Cautions { get; set; }

        [JsonPropertyName("ingredients")]
        public List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("parsed")]
        public List<Parsed> Parsed { get; set; }
    }

    public class Parsed
    {
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("measure")]
        public string Measure { get; set; }

        [JsonPropertyName("foodMatch")]
        public string FoodMatch { get; set; }

        [JsonPropertyName("food")]
        public string Food { get; set; }

        [JsonPropertyName("foodId")]
        public string FoodId { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        [JsonPropertyName("retainedWeight")]
        public double RetainedWeight { get; set; }

        [JsonPropertyName("nutrients")]
        public Dictionary<string, Nutrient> Nutrients { get; set; }

        [JsonPropertyName("measureURI")]
        public string MeasureURI { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Nutrient
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }
    }

}
