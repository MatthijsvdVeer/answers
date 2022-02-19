using System;
using System.Device.Location;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace QA742102
{
    class Program
    {
        private static DeviceClient deviceClient;

        static void Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString("<YOUR DEVICE CONNECTION STRING>");

            var watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += WatcherOnPositionChangedAsync;
            var started = watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));
            if (!started)
            {
                Console.WriteLine("Out of luck.");
                return;
            }

            Console.WriteLine("Enter any key to quit.");
            Console.ReadLine();
        }

        private static async void WatcherOnPositionChangedAsync(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Console.WriteLine(e.Position.Location.ToString());
            var json = JsonConvert.SerializeObject(e.Position);
            var eventMessage = new Message(Encoding.UTF8.GetBytes(json))
            {
                ContentEncoding = Encoding.UTF8.ToString(),
                ContentType = "application/json",
            };

            await deviceClient.SendEventAsync(eventMessage);
        }
    }
}