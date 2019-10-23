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

        private static readonly Regex AddressPortRegex = new Regex(@"^\[?([^\]]*)]?:([\d*]*)$");
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
        /// /// <param name="pidColumnNumber">the number of the column that contains PID</param>
        /// <returns>A list of information per each "LISTEN" socket</returns>
        public static List<ListeningPortsPayload> ParseNetstatListeners(string content, int localColumn, int remoteColumn, int pidColumnNumber)
        {
            List<ListeningPortsPayload> netstatListeners = new List<ListeningPortsPayload>();

            string[] rows = content.SplitStringOnNewLine();
            foreach (string row in rows)
            {
                string[] tokens = Regex.Split(row.Trim(), "\\s+");
                string token = tokens[0];

                if (Protocols.Contains(token, StringComparer.InvariantCultureIgnoreCase))
                {
                    ListeningPortsPayload listeningPorts = new ListeningPortsPayload();
                    //Set protocol
                    listeningPorts.Protocol = token.ToLower();

                    //Set local address IP and port:
                    var local = AddressPortRegex.Match(tokens[localColumn]);
                    listeningPorts.LocalAddress = local.Groups[1].Value;
                    listeningPorts.LocalPort = local.Groups[2].Value;

                    //Set foreign address IP and port:
                    var remote = AddressPortRegex.Match(tokens[remoteColumn]);
                    listeningPorts.RemoteAddress = remote.Groups[1].Value;
                    listeningPorts.RemotePort = remote.Groups[2].Value;

                    // ExtraDetails will include all extracted non-schema properties
                    var extraDetails = new Dictionary<string, string>();

                    int maxOffset = pidColumnNumber;
                    string pid;

                    // Extract pid
                    if (tokens.Length > maxOffset)
                    {
                        pid = tokens[pidColumnNumber];
                    }
                    else
                    {
                        // state is empty
                        pid = tokens[pidColumnNumber - 1];
                    }

                    if (!string.IsNullOrWhiteSpace(pid))
                    {
                        Match pidMatch = Regex.Match(pid, @"^\d+");

                        if (pidMatch.Captures.Count > 0 && !string.IsNullOrWhiteSpace(pidMatch.Value))
                        {
                            extraDetails["Pid"] = pidMatch.ToString();
                        }
                    }

                    if (extraDetails.Count > 0)
                    {
                        listeningPorts.ExtraDetails = extraDetails;
                    }

                    netstatListeners.Add(listeningPorts);
                }
            }
            return netstatListeners;
        }
    }
}