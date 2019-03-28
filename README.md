<img src="https://github.com/Azure/IoT-ATP/blob/master/logo/logo.svg" align="right"
     title="Azure Security Center for IoT" width="100" height="158">
    
# Azure Security Center for IoT

## Security Agent - C#

ATP for IoT provides reference architecture for security agents that log, process, aggregate, and send security data through IoT hub.
Security agents are designed to work in a constrained IoT environment, and are highly customizable in terms of values they provide when compared to the resources they consume.

Security agents support the following IoT solution features:
- Collect raw security events from the underlying OS (Linux, Windows). To learn more about available security data collectors, see ATP for IoT agent configuration.
- Aggregate raw security events into messages sent through IoT hub.
- Authenticate with existing device identity, or a dedicated module identity. See Security agent authentication methods to learn more.
- Configure remotely through use of the atpforiot module twin. To learn more, see Configure an ATP for IoT agent.

In this repository, you will find source code and binaries of C#-based security agents for [Linux](https://aka.ms/iot-security-docs-agent-cs-linux) and [Windows](https://aka.ms/iot-security-docs-agent-cs-win).

See also,

- ASC for IoT on [Azure Docs](https://docs.microsoft.com/en-us/azure/asc-for-iot/concepts-security-agent)
- ASC for IoT samples repository, on [Github](https://aka.ms/iot-security-github-samples)
- ASC for IoT security agent reference architecture for C, on [Github](https://aka.ms/iot-security-github-c)

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.