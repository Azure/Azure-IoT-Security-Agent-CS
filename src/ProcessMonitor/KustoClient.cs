using System;
using System.IO;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProcessMonitor
{
    public class MessageSender
    {
        private const string db = "DevRIOTAlerts";
        private const string table = "AgentPerfData";
        private const string clusterName = "rome";
        private const string mappingName = "AgentPerfData_mapping_1";
        private const string applicationId = "d31f5be0-eaf3-425a-9f6f-e6c70e1a96f3";        
        private const string microsoftAuthority = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        private static readonly string applicationKey = Configuration.AppKey;

        public static void SendMessage(object perfData)
        {
            var ingestionConnectionBuilder = new KustoConnectionStringBuilder($"https://ingest-{clusterName}.kusto.windows.net").WithAadApplicationKeyAuthentication(
                applicationId,
                applicationKey,
                microsoftAuthority);

            using (var ingestClient = KustoIngestFactory.CreateQueuedIngestClient(ingestionConnectionBuilder))
            {
                var ingestProps = new KustoQueuedIngestionProperties(db, table);
                ingestProps.FlushImmediately = true;
                ingestProps.JSONMappingReference = mappingName;
                ingestProps.Format = DataSourceFormat.json;

                string messageJson = JsonConvert.SerializeObject(perfData);

                Console.WriteLine($"sending data: {messageJson} ");

                using (var memStream = new MemoryStream())
                using (var writer = new StreamWriter(memStream))
                {
                    writer.Write(messageJson);
                    writer.Flush();
                    memStream.Seek(0, SeekOrigin.Begin);
                    ingestClient.IngestFromStream(memStream, ingestProps, leaveOpen: true);
                }
            }
        }

        public static void CreateKustoTable()
        {
            var builder = new KustoConnectionStringBuilder($"https://{clusterName}.kusto.windows.net").WithAadApplicationKeyAuthentication(
                applicationId,
                applicationKey,
                microsoftAuthority);

            builder.InitialCatalog = db;
            builder.FederatedSecurity = true;

            using (var kustoAdminClient = KustoClientFactory.CreateCslAdminProvider(builder))
            {
                var columns = new List<Tuple<string, string>>()
                {                    
                    new Tuple<string, string>($"{nameof(AgentPerfData.Host)}", "System.String"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.SystemTime)}", "System.DateTime"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.ProcessName)}", "System.String"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.NumberOfProcesses)}", "System.Int32"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.ProcessId)}", "System.Int32"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.CpuPercentage)}", "System.Double"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.Memory)}", "System.Double"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.MemoryPercentage)}", "System.Double"),
                    new Tuple<string, string>($"{nameof(AgentPerfData.AdditionalData)}", "System.String"),
                };

                var command = CslCommandGenerator.GenerateTableCreateCommand(table, columns);
                kustoAdminClient.ExecuteControlCommand(command);

                // Set up mapping
                var columnMappings = new List<JsonColumnMapping>();
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.Host)}", JsonPath = $"$.{nameof(AgentPerfData.Host)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.SystemTime)}", JsonPath = $"$.{nameof(AgentPerfData.SystemTime)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.ProcessName)}", JsonPath = $"$.{nameof(AgentPerfData.ProcessName)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.NumberOfProcesses)}", JsonPath = $"$.{nameof(AgentPerfData.NumberOfProcesses)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.ProcessId)}", JsonPath = $"$.{nameof(AgentPerfData.ProcessId)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.CpuPercentage)}", JsonPath = $"$.{nameof(AgentPerfData.CpuPercentage)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.Memory)}", JsonPath = $"$.{nameof(AgentPerfData.Memory)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.MemoryPercentage)}", JsonPath = $"$.{nameof(AgentPerfData.MemoryPercentage)}" });
                columnMappings.Add(new JsonColumnMapping()
                { ColumnName = $"{nameof(AgentPerfData.AdditionalData)}", JsonPath = $"$.{nameof(AgentPerfData.AdditionalData)}" });


                command = CslCommandGenerator.GenerateTableJsonMappingCreateCommand(
                                                    table, mappingName, columnMappings);
                kustoAdminClient.ExecuteControlCommand(command);
            }
        }
       
    }
}
