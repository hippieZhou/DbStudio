using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Versioning;

namespace DbStudio.Infrastructure.Shared.Helpers
{
    public class MachineHelper
    {
        [SupportedOSPlatform("windows")]
        public static string GetMachineName()
        {
            using var searcher = new ManagementObjectSearcher(new SelectQuery("Win32_ComputerSystem"));
            foreach (var mo in searcher.Get())
            {
                if ((bool)mo["PartOfDomain"])
                    return mo["DNSHostName"] + "." + mo["domain"];
            }

            return Environment.MachineName;
        }


        public static string GetMacAddress()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return "NetworkNotReady";

            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var adapter =
                nics.FirstOrDefault(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ??
                nics.FirstOrDefault(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            return adapter != null ? adapter.GetPhysicalAddress().ToString() : "NotFound";
        }

        public static string GetIpAddress()
        {
            //获取说有网卡信息
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
                //判断是否为以太网卡
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    var ip = adapter.GetIPProperties();
                    //获取单播地址集
                    var ipCollection = ip.UnicastAddresses;
                    foreach (var ipadd in ipCollection)
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            //判断是否为ipv4
                            return ipadd.Address.ToString(); //获取ip
                }

            return "Unknown";
        }

        public static string GetOsVersion() => Environment.OSVersion.ToString();

        public static string GetUserName() => Environment.UserName;
    }
}