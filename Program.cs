
namespace network_traffic_dynamic_icon.app
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplicationContext());
        }
    }
}