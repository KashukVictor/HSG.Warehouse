using HSG.Warehouse.Common.Models.Entity.Base;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HSG.Warehouse.Common.Models.Entity
{
    public class Currency : BaseModel
    {
        [JsonPropertyName("exchangedate"),
            JsonConverter(typeof(CustomDateConvert))]
        public DateTime? Date { get; set; }
        [JsonPropertyName("rate")]
        public double RateNBU { get; set; }
        [JsonPropertyName("r030")]
        public int Code { get; set; }
        [JsonPropertyName("txt")]
        public string? Name { get; set; }
        [JsonPropertyName("cc")]
        public string? ShortName { get; set; }


        private class CustomDateConvert : JsonConverter<DateTime>
        {
            private const string dateFormat = "dd.MM.yyyy";
            public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
            {
                writer.WriteStringValue(date.ToString(dateFormat));
            }
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.ParseExact(reader.GetString(), dateFormat, null);
            }
        }

    }


}