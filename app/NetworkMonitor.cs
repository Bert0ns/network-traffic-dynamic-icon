using System.Net.NetworkInformation;


namespace network_traffic_dynamic_icon.app
{
    public class NetworkMonitor
    {
        private long _prevBytesReceived;
        private long _prevBytesSent;
        private DateTime _prevTime;
        private bool _initialized = false;

        public void ResetForInterface(NetworkInterface nic)
        {
            // Usa le statistiche IP generiche (IPv4 + IPv6) invece delle sole IPv4
            var stats = nic.GetIPStatistics();
            _prevBytesReceived = stats.BytesReceived;
            _prevBytesSent = stats.BytesSent;
            _prevTime = DateTime.UtcNow;
            _initialized = true;
        }

        public (long downBps, long upBps) GetCurrentSpeed(NetworkInterface nic)
        {
            if (!_initialized)
            {
                ResetForInterface(nic);
                return (0, 0);
            }

            var stats = nic.GetIPStatistics(); // include traffico IPv4 + IPv6
            var now = DateTime.UtcNow;
            double seconds = (now - _prevTime).TotalSeconds;
            if (seconds <= 0) return (0, 0);

            long deltaDown = stats.BytesReceived - _prevBytesReceived;
            long deltaUp = stats.BytesSent - _prevBytesSent;

            _prevBytesReceived = stats.BytesReceived;
            _prevBytesSent = stats.BytesSent;
            _prevTime = now;

            if (deltaDown < 0 || deltaUp < 0)
                return (0, 0); // contatori resettati?

            long downBps = (long)(deltaDown / seconds);
            long upBps = (long)(deltaUp / seconds);
            return (downBps, upBps);
        }
    }
}
