using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

        // Set the URL for the server
        var url = "http://localhost:8011/water_data"; // Replace with your server's URL

        // Create an HttpClient to send requests
        using var client = new HttpClient();

        // Generate sensor data and send it every second
        while (true)
        {
            // Simulating sensor data
            var flowRate = new Random().Next(0, 100);  // Random flow rate (0 to 100)
            var waterTemperature = new Random().Next(20, 40); // Random temperature (20 to 40)

            // Create the data object to send
            var data = new
            {
                flowRate = flowRate,
                waterTemperature = waterTemperature
            };

            // Convert the data object to JSON
            var jsonData = JsonSerializer.Serialize(data);

            // Send the data to the server
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            // Post the data to the server
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Data sent successfully: " + jsonData);
            }
            else
            {
                Console.WriteLine("Error sending data: " + response.StatusCode);
            }

            // Wait for 1 second before sending the next data
            Thread.Sleep(1000);
        }
 
