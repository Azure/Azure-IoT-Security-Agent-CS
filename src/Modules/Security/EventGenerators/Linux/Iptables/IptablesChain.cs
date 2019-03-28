using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Iptables
{
    class IptablesChain
    {
        /// <summary>
        /// This chain firewall rules
        /// </summary>
        public IList<IptableRule> Rules { get; }

        /// <summary>
        /// Chain name
        /// </summary>
        public string Name { get; }

        private static readonly Regex ChainRegex = new Regex(@":([^\s]*)\s([^\s]*)");
        private static readonly Regex ChainNamePolicy = new Regex(@"^:([^\s]*)");
        private static readonly Regex ChainNameRule = new Regex(@"^-A ([^\s]*)");

        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="name">Chain name</param>
        /// <param name="rules">Chain rules</param>
        public IptablesChain(string name, IList<IptableRule> rules)
        {
            Rules = rules;
            Name = name;
        }

        /// <summary>
        /// Gets an array of chain specific rules from iptables-save output 
        /// and parse them as an IptableChain
        /// Chain specific rules, are all the rules (include policy) of the same chain
        /// </summary>
        /// <param name="chain">Chain specific rules</param>
        /// <returns>IptablesChain</returns>
        public static IptablesChain ParseChain(string[] chain)
        {
            var rules = new List<IptableRule>();
            string chainHeaderLine = chain.First(l => l.StartsWith(":")); //Get's the chain header line ":<ChainName> <defaultaction> [bytesin:bytesou]

            int priority = 0;
            foreach (var line in chain)
            {
                if (line.StartsWith("-")) 
                {
                    rules.Add(IptableRule.ParseRuleFromIptablesSaveLine(line, priority));
                    priority++;
                }
            }

            var name = GetChainNameFromLine(chainHeaderLine);
            var defaultPolicyRule = GetChainDefaultPolicyRule(chainHeaderLine, priority);
            if (defaultPolicyRule != null)
            {
                rules.Add(GetChainDefaultPolicyRule(chainHeaderLine, priority));
            }

            return new IptablesChain(name, rules);
        }

        /// <summary>
        /// Parses the given iptables filter table to IptableChain according to the table content
        /// </summary>
        /// <param name="filterTable">iptable filterTable output (from iptable-save)</param>
        /// <returns>return list of parsed chains</returns>
        public static List<IptablesChain> GetChainsFromTable(string[] filterTable)
        {
            return filterTable.GroupBy(GetChainNameFromLine)
                .Where(key => !string.IsNullOrEmpty(key.Key))
                .Select(chain => ParseChain(chain.ToArray()))
                .ToList();
        }

        private static string GetChainNameFromLine(string lineContent)
        {
            if (ChainNamePolicy.IsMatch(lineContent))
            {
                return ChainNamePolicy.Match(lineContent).Groups[1].Value;
            }
            if (ChainNameRule.IsMatch(lineContent))
            {
                return ChainNameRule.Match(lineContent).Groups[1].Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates a rule based on the chain's default action (policy)
        /// </summary>
        /// <param name="chainHeaderLine">The chain policy line output from iptables-save</param>
        /// <param name="priority">Rule priority</param>
        /// <returns>the chain's policy (default) rule</returns>
        private static IptableRule GetChainDefaultPolicyRule(string chainHeaderLine, int priority)
        {
            var actionString = ChainRegex.Matches(chainHeaderLine)[0].Groups[2].Value;

            if (actionString == "-")
            {
                return null;
            }

            return new IptableRule(priority, IptableRule.ParseAction(actionString), null, null);
        }
    }
}
