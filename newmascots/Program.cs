using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "../Style.ultra";

        var lines = File.ReadAllLines(filePath);

        bool inPreamble = true;

        var mascots = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "            - - match")
                break;

            if (inPreamble)
            {
                if (lines[i] == "              - [ \"get\", \"mascot\" ]")
                {
                    inPreamble = false;
                }
                continue;
            }

            if (lines[i].StartsWith("            - - "))
            {
                mascots.Add(lines[i].Replace("            - - ", ""));
            }
            else if (lines[i].StartsWith("              - "))
            {
                mascots.Add(lines[i].Replace("              - ", ""));
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
