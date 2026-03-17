using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "../Style.ultra";

        var lines = File.ReadAllLines(filePath);

        var mascots = new List<string>();

        // Find the first "- [ "get", "mascot" ]" to skip the preamble,
        // then collect mascot names until "- - match" (the letterman fallback)
        int start = 0;
        for (; start < lines.Length; start++)
        {
            if (lines[start].Trim() == "- [ \"get\", \"mascot\" ]")
                break;
        }

        for (int i = start + 1; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();

            if (trimmed == "- - match")
                break;

            if (trimmed.StartsWith("- - ") && char.IsUpper(trimmed[4]))
            {
                mascots.Add(trimmed.Substring(4));
            }
            else if (trimmed.StartsWith("- ") && trimmed.Length > 2 && char.IsUpper(trimmed[2]))
            {
                mascots.Add(trimmed.Substring(2));
            }
        }


        string apiUrl = "https://taginfo.openstreetmap.org/api/4/key/values?key=mascot";
        // Call the function to fetch and compare results
        List<string> newResults = await FetchAndReturnValues(apiUrl);
        PrintNewItems(newResults, mascots);
    }

    static async Task<List<string>> FetchAndReturnValues(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            var values = new List<string>();

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON and extract only "value" fields
                var jsonData = System.Text.Json.JsonDocument.Parse(responseBody);
                var dataArray = jsonData.RootElement.GetProperty("data");

                foreach (var item in dataArray.EnumerateArray())
                {
                    if (item.TryGetProperty("value", out var value))
                    {
                        values.Add(value.GetString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return values;
        }
    }
    static void PrintNewItems(List<string> newResults, List<string> oldResults)
    {
        string fileName = "../NewMascots.txt";

        try
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (var newItem in newResults)
                {
                    if (!oldResults.Contains(newItem))
                    {
                        writer.WriteLine(newItem);
                    }
                }
            }

            Console.WriteLine($"New items have been written to {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }
}
