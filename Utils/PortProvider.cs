using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Utils
{
    public static class PortProvider
    {
        public static int GetAvailablePort()
        {
            const int minPort=49152; 
            const int maxPort=65535;

            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var busyPort = tcpConnInfoArray.Select(conn => conn.LocalEndPoint.Port).ToArray();
            
            for (var port = minPort; port <= maxPort; port++)
            {
                if (!busyPort.Contains(port))
                {
                    return port;
                }
            }

            throw new ArgumentException("Can't find available Port for Client");
        }

    }
}