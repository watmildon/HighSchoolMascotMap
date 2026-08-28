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

        if (newResults.Count == 0)
        {
            Console.Error.WriteLine("No mascot values were returned from TagInfo. Refusing to overwrite NewMascots.txt.");
            Environment.Exit(1);
        }

        PrintNewItems(newResults, mascots);
    }

    static async Task<List<string>> FetchAndReturnValues(string url)
    {
        // TagInfo rejects unpaged requests for keys with many values (HTTP 412,
        // "number of results too large, use paging"), so walk the pages.
        const int resultsPerPage = 999;

        using (HttpClient client = new HttpClient())
        {
            var values = new List<string>();
            int total = 0;

            for (int page = 1; ; page++)
            {
                string pagedUrl = $"{url}&rp={resultsPerPage}&page={page}";

                HttpResponseMessage response = await client.GetAsync(pagedUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON and extract only "value" fields
                var jsonData = System.Text.Json.JsonDocument.Parse(responseBody);

                if (jsonData.RootElement.TryGetProperty("total", out var totalElement))
                {
                    total = totalElement.GetInt32();
                }

                var dataArray = jsonData.RootElement.GetProperty("data");
                int countThisPage = 0;

                foreach (var item in dataArray.EnumerateArray())
                {
                    countThisPage++;

                    if (item.TryGetProperty("value", out var value))
                    {
                        string? mascotValue = value.GetString();

                        if (mascotValue != null)
                        {
                            values.Add(mascotValue);
                        }
                    }
                }

                if (countThisPage == 0 || values.Count >= total)
                {
                    break;
                }
            }

            if (values.Count < total)
            {
                throw new Exception($"Expected {total} mascot values from TagInfo but only retrieved {values.Count}.");
            }

            Console.WriteLine($"Retrieved {values.Count} mascot values from TagInfo.");

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
            Console.Error.WriteLine($"Error writing to file: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
