using System.Net.NetworkInformation;
using Timer = System.Windows.Forms.Timer;


namespace network_traffic_dynamic_icon.app
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Timer _uiTimer;
        private readonly NetworkMonitor _monitor;
        private NetworkInterface? _selectedInterface;
        private Icon? _currentDynamicIcon;
        private int _iconUpdateCounter = 0;

        public TrayApplicationContext()
        {
            _monitor = new NetworkMonitor();

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Inizializzazione rete..."
            };

            // Crea menu
            var menu = new ContextMenuStrip();
            menu.Items.Add("Seleziona interfaccia"); // Placeholder, verrà sostituito
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Esci", null, (_, _) => ExitApplication());


            var autoStartItem = new ToolStripMenuItem("Avvio automatico all'accesso")
            {
                Checked = AutoStartManager.IsEnabled()
            };
            autoStartItem.Click += (_, _) =>
            {
                if (AutoStartManager.IsEnabled())
                {
                    AutoStartManager.Disable();
                    autoStartItem.Checked = false;
                }
                else
                {
                    AutoStartManager.Enable();
                    autoStartItem.Checked = true;
                }
            };
            menu.Items.Insert(1, autoStartItem);


            _notifyIcon.ContextMenuStrip = menu;
            BuildInterfaceMenu(menu); // Popola interfacce

            // Timer UI (WinForms) ogni 1000 ms
            _uiTimer = new Timer { Interval = 1000 };
            _uiTimer.Tick += (_, _) => UpdateStats();
            _uiTimer.Start();
        }

        private void BuildInterfaceMenu(ContextMenuStrip menu)
        {
            // Rimuove voce "Seleziona interfaccia" precedente se esiste
            var old = menu.Items
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(i => i.Text == "Seleziona interfaccia");
            if (old != null)
                menu.Items.Remove(old);

            var sub = new ToolStripMenuItem("Seleziona interfaccia");

            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()
                         .Where(n =>
                             n.OperationalStatus == OperationalStatus.Up &&
                             n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                             n.NetworkInterfaceType != NetworkInterfaceType.Tunnel))
            {
                var item = new ToolStripMenuItem(nic.Name)
                {
                    Checked = _selectedInterface?.Id == nic.Id
                };
                item.Click += (_, _) =>
                {
                    _selectedInterface = nic;
                    _monitor.ResetForInterface(nic);
                    foreach (ToolStripMenuItem it in sub.DropDownItems)
                        it.Checked = false;
                    item.Checked = true;
                };
                sub.DropDownItems.Add(item);

                // Se non selezionata ancora nessuna, prende la prima
                _selectedInterface ??= nic;
            }

            menu.Items.Insert(0, sub);

            if (_selectedInterface != null)
                _monitor.ResetForInterface(_selectedInterface);
        }

        private void UpdateStats()
        {
            if (_selectedInterface == null)
            {
                _notifyIcon.Text = "Nessuna interfaccia attiva";
                return;
            }

            var (downBps, upBps) = _monitor.GetCurrentSpeed(_selectedInterface);

            string downTxt = FormatSpeed(downBps);
            string upTxt = FormatSpeed(upBps);

            // Tooltip (max ~63 caratteri su alcune versioni)
            _notifyIcon.Text = $"Down: {downTxt} | Up: {upTxt}";

            // Aggiorna icona ogni 3 secondi per ridurre consumo GDI
            _iconUpdateCounter++;
            if (_iconUpdateCounter % 3 == 0)
            {
                var newIcon = IconFactory.MakeIcon(downBps, upBps);
                // Rilascia icona precedente per evitare leak
                _notifyIcon.Icon = newIcon;
                _currentDynamicIcon?.Dispose();
                _currentDynamicIcon = newIcon;
            }
        }

        private static string FormatSpeed(long bytesPerSec)
        {
            if (bytesPerSec < 0) return "0 B/s";
            double v = bytesPerSec;
            string[] units = { "B/s", "KB/s", "MB/s", "GB/s" };
            int idx = 0;
            while (v >= 1024 && idx < units.Length - 1)
            {
                v /= 1024;
                idx++;
            }
            return v < 10 ? $"{v:0.0} {units[idx]}" : $"{v:0} {units[idx]}";
        }

        private void ExitApplication()
        {
            _uiTimer.Stop();
            _notifyIcon.Visible = false;
            _currentDynamicIcon?.Dispose();
            _notifyIcon.Dispose();
            Application.Exit();
        }
    }
}
