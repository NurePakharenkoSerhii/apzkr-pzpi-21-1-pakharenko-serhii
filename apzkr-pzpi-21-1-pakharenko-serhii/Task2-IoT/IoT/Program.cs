using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using IoT.Entities;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = Encoding.UTF8;

ConfigurationBuilder builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
IConfiguration configBuilder = builder.Build();
Configuration configuration = new Configuration();
configBuilder.Bind(configuration);

Greetings greetings = new Greetings();
greetings.GreetUser();

using HttpClient client = new HttpClient();
var loginData = new
{
    email = configuration.Credentials.Email,
    password = configuration.Credentials.Password,
};

StringContent loginContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
var loginResponse = await client.PostAsync($"{configuration.ApiUrl}/User/login", loginContent);

if (!loginResponse.IsSuccessStatusCode)
{
    Console.WriteLine($"Error {loginResponse.StatusCode} - Impossible to login.");
    return;
}

var jwtToken = await loginResponse.Content.ReadAsStringAsync();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

greetings.AskForIdCard();

var userId = Convert.ToInt32(Console.ReadLine());

greetings.CheckingUserExistence();
greetings.LoadingLine();

try
{
    var userResponse = await client.GetAsync($"{configuration.ApiUrl}/User/{userId}");

    if (!userResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"User not found! Error: {userResponse.StatusCode}");
        return;
    }
    
    var userResponseBody = await userResponse.Content.ReadAsStringAsync();
    var user = JsonSerializer.Deserialize<User>(userResponseBody);

    Console.WriteLine($"Hello there, {user.fullname}!");
    greetings.GettingInventoryMessage();
    greetings.LoadingLine();
    
    var response = await client.PostAsync($"{configuration.ApiUrl}/Thing/takeThings/{userId}", null);
    
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        return;
    }

    var responseBody = await response.Content.ReadAsStringAsync();
    var orders = JsonSerializer.Deserialize<AssignedThings>(responseBody)!.assignedThingDtos;
    
    greetings.ShowSuccessMessage();
    
    foreach (var order in orders)
    {
        Console.WriteLine($"Assignment ID: {order.assignmentId}" +
                          $"Order: {order.orderId}" +
                          $"Amount: {order.amount}" +
                          $"Frequency: {order.frequency}" +
                          $"You can keep thing for: {order.duration}" +
                          $"Date assigned: {order.dateAssignedUTC}");
        greetings.LoadingLine();
    }
    greetings.LoadingLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Some exception occured: {ex.Message}");
}
