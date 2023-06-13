using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HSG.Warehouse.ClientServices.NbuCurrency
{
    public class NbuCurrencyServiceClient : INbuCurrencyServiceClient
    {
        private readonly HttpClient _httpClient;

        public NbuCurrencyServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Currency>?> GetCurrencyOnDateAsync(DateTime date)
        {
            var URL = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={date.ToString("yyyyMMdd")}&json";
            var respons = await _httpClient.GetStringAsync(URL);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = JsonSerializer.Deserialize<IEnumerable<Currency>>(respons, options);

            return result;


        }
    }
}
