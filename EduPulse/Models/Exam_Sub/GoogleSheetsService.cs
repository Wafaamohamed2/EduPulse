using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class GoogleSheetsService
{
    private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    private static readonly string ApplicationName = "Google Sheets API .NET Quickstart";
    private static SheetsService service;

    public static async Task InitializeGoogleSheetsApi()
    {
        UserCredential credential;

        // The file token.json stores the user's access and refresh tokens.
        string credPath = "token.json";

        // Load the credentials from the client_secret.json file
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }

        // Create Google Sheets API service
        service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public static async Task<int> GetGradeFromGoogleSheet(string spreadsheetId, int row)
    {
        // Read data from the Google Sheets (assuming grades are stored in the first column)
        var range = $"Sheet1!A{row}:B{row}";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();

        // Assuming grade is in the second column (B)
        var grade = response.Values[0][1].ToString();
        return int.Parse(grade);
    }
}
 