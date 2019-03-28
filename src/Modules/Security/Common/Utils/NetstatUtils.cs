// <copyright file="NetstatUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Microsoft.Azure.Security.IoT.Agent.Common.Utils
{
    /// <summary>
    /// Helper class for parsing the output of netstat commands
    /// </summary>
    public static class NetstatUtils
    {
        /// <summary>
        /// Prtocol to parse
        /// </summary>
        private enum NetstatProtocols
        {
            Tcp,
            Tcp6,
            Udp,
            Udp6,
            Raw,
            Raw6
        }

        private static readonly string[] Protocols = Enum.GetNames(typeof(NetstatProtocols));
        private static readonly Regex AdressPortRegex = new Regex("^(.*):([\\d*]*)$");
        /// <summary>
        /// The method receieves an output of 'netstat' command
        /// It parse only the rows in "LISTEN" state
        /// Returns a list of information per each "LISTEN" socket
        /// Implementation remark:
        /// We can't use the .NET API GetActiveTcpListeners since this API returns listeners in all TCP states except the Listen state.
        /// That's why we have to parse the netstat output
        /// </summary>
        /// <param name="content">the output of 'netstat -nlpt' command as expected in Linux</param>
        /// /// <param name="localColumn">the number of the column that contains local addresses</param>
        /// /// <param name="remoteColumn">the number of the column that contains remote addresses</param>
        /// <returns>A list of information per each "LISTEN" socket</returns>
        public static List<ListeningPortsPayload> ParseNetstatListeners(string content, int localColumn, int remoteColumn)
        {
            List<ListeningPortsPayload> netstatListeners = new List<ListeningPortsPayload>();

            string[] rows = content.SplitStringOnNewLine();
            foreach (string row in rows)
            {
                string[] tokens = Regex.Split(row.Trim(), "\\s+");
                string protocol = tokens[0];
                if (Protocols.Contains(protocol, StringComparer.InvariantCultureIgnoreCase))
                {
                    ListeningPortsPayload listeningPorts = new ListeningPortsPayload();
                    //Set protocol
                    listeningPorts.Protocol = tokens[0].ToLower();

                    //Set local address IP and port:
                    var local = AdressPortRegex.Match(tokens[localColumn]);
                    listeningPorts.LocalAddress = local.Groups[1].Value;
                    listeningPorts.LocalPort = local.Groups[2].Value;

                    //Set foreign address IP and port:
                    var remote = AdressPortRegex.Match(tokens[remoteColumn]);
                    listeningPorts.RemoteAddress = remote.Groups[1].Value;
                    listeningPorts.RemotePort = remote.Groups[2].Value;
                    netstatListeners.Add(listeningPorts);
                }
            }
            return netstatListeners;
        }
    }
}