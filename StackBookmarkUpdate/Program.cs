using System.Text.Json;

Console.WriteLine("Enter stack number ###.");
string? input = Console.ReadLine();
string stackName = string.Empty;

if (input == null || !int.TryParse(input,out int stackNumber))
{
    Console.WriteLine("Please enter an integer stack number ###.");
    return;
}

string directoryPath = @"C:\Users\amargraff\AppData\Local\Google\Chrome\User Data\Default";
string fileName = "Bookmarks";
string filePath = Path.Combine(directoryPath, fileName);

try
{
    // Check if the file exists
    if (File.Exists(filePath))
    {
        // Read the JSON string from the file
        string jsonString = File.ReadAllText(filePath);
        try
        {
            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            JsonElement root = jsonDocument.RootElement;
            root.TryGetProperty("roots", out JsonElement roots);
            roots.TryGetProperty("bookmark_bar", out JsonElement bookmark_bar);
            bookmark_bar.TryGetProperty("children", out JsonElement children);

            if (children.ValueKind == JsonValueKind.Array)
            {
                var folder = children
                    .EnumerateArray()
                    .Where(c => c.TryGetProperty("name", out JsonElement folderName) && folderName.GetString() == "Vidapay-Stack")
                    .FirstOrDefault();

                if (folder.TryGetProperty("name", out JsonElement folderName) && folderName.GetString() == "Vidapay-Stack")
                {
                    folder.TryGetProperty("children", out JsonElement bookmarks);
                    JsonElement firstBookmark = bookmarks.EnumerateArray().FirstOrDefault();

                    firstBookmark.TryGetProperty("url", out JsonElement url);
                    string? urlString = url.GetString();
                    if (urlString != null)
                    {
                        int indexOfStack = 0;
                        int indexOfPeriod = 0;
                        indexOfStack = urlString.IndexOf("stack");
                        if (indexOfStack != -1)
                        {
                            indexOfPeriod = urlString.IndexOf('.', indexOfStack);
                            if (indexOfPeriod != -1)
                            {
                                stackName = urlString.Substring(indexOfStack, indexOfPeriod - indexOfStack);
                            }
                            else { Console.WriteLine("Stack Name Not Found"); }
                        }
                        else { Console.WriteLine("Stack Name Not Found"); }
                    }
                }
            }

            if (!string.IsNullOrEmpty(stackName))
            {
                Console.WriteLine($"Replacing {stackName} with stack{stackNumber}. Press any key to continue.");
                Console.ReadKey();
                jsonString = jsonString.Replace(stackName, $"stack{stackNumber}", StringComparison.InvariantCultureIgnoreCase);
                File.WriteAllText(filePath, jsonString);
            }
            else
            {
                Console.WriteLine("Could not find stack name to replace.");
            }

            jsonDocument.Dispose();
        }
        catch (Exception ex){ Console.WriteLine("An error occurred: " + ex.Message);}
    }
    else
    {
        Console.WriteLine("File does not exist: " + filePath);
    }
}
catch (Exception ex){Console.WriteLine("An error occurred: " + ex.Message); }

