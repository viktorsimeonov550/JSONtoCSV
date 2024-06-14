using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml.Schema;
using JSONtoCSV;

namespace JSONtoCSV;

public static class JsonToCsv
{
    private static Country country = new Country();

    public static void CheckUserInput(string userInput)
    {
        string userInputValidator = @"^(?<path>\w+)/\w+$";

        Match match = Regex.Match(userInput, userInputValidator);

        if (!match.Success)
        {
            throw new Exception("invalid input request");
        }
    }

    public static string GetJson(string userInput)
    {
        Task<HttpResponseMessage> response = GetResponse(userInput);
        HttpStatusCode statusCode = response.Result.StatusCode;

        if (statusCode == HttpStatusCode.NotFound)
        {
            throw new Exception("No results found");
        }
        else if (statusCode != HttpStatusCode.OK)
        {
            throw new Exception("An error appeared! Try again.");
        }

        string responseAsString = ReadResponseAsStringAsync(response);

        try
        {
            country = DeserializeJson(responseAsString);
        }
        catch (Exception)
        {
            throw new Exception("Invalid JSON. Try another request.");
        }

        string countriesAsJson = SerializeJson();

        return countriesAsJson;
    }

    public static Task<HttpResponseMessage> GetResponse(string userInput)
    {
        string url = $"https://api.zippopotam.us/{userInput}";

        HttpClient client = new HttpClient();
        Task<HttpResponseMessage> response = client.GetAsync(url);

        return response;
    }

    public static string ReadResponseAsStringAsync(Task<HttpResponseMessage> response)
        => response.Result.Content.ReadAsStringAsync().Result;

    public static Country DeserializeJson(string responseAsString)
        => JsonConvert.DeserializeObject<Country>(responseAsString);

    public static string SerializeJson()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        string serializedJson = JsonConvert
            .SerializeObject(country, Formatting.Indented, settings);

        return serializedJson;
    }

    public static string GetCsv()
    {
        StringWriter writer = new StringWriter();
        CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteField("post code");
        csv.WriteField("country");
        csv.WriteField("country abbreviation");
        csv.WriteField("places/place name");
        csv.WriteField("places/longitude");
        csv.WriteField("places/state");
        csv.WriteField("places/state abbreviation");
        csv.WriteField("places/latitude");
        csv.NextRecord();

        foreach (CountryPlaces place in country.Places)
        {
            csv.WriteField(country.PostCode);
            csv.WriteField(country.CountryName);
            csv.WriteField(country.CountryAbbr);

            csv.WriteField(place.PlaceName);
            csv.WriteField(place.Longitude);
            csv.WriteField(place.State);
            csv.WriteField(place.StateAbbr);
            csv.WriteField(place.Latitude);

            csv.NextRecord();
        }

        return writer.ToString();
    }
}
