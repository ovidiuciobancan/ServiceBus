using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;


namespace OC.ServiceBus.Extensions
{
    public static class Extensions
    {
        public static async Task RemoveDefaultFiltersIfExists(this SubscriptionClient subscription)
        {
            var rules = await subscription.GetRulesAsync();
            if(rules.Any(r => r.Name == RuleDescription.DefaultRuleName))
            {
                await subscription.RemoveRuleAsync(RuleDescription.DefaultRuleName);
            }
        }

        public static async Task AddFilterAsyc(this SubscriptionClient subscription, RuleDescription rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            var rules = await subscription.GetRulesAsync();

            if (!rules.Any(r => r.Name == rule.Name))
            {
                await subscription.AddRuleAsync(rule);
            }
        }

        public static async Task RemoveFilterAsyc(this SubscriptionClient subscription, string ruleName)
        {
            if (String.IsNullOrWhiteSpace(ruleName)) throw new ArgumentNullException(nameof(ruleName));

            var rules = await subscription.GetRulesAsync();

            if (rules.Any(r => r.Name == ruleName))
            {
                await subscription.RemoveRuleAsync(ruleName);
            }
        }
    }
}
