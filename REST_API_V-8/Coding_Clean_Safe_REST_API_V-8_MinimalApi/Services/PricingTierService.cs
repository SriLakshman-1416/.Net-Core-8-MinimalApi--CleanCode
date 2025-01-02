using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Enums;
using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Interfaces;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Services;

public class PricingTierService : IPricingTierService
{
    public PricingTier GetPricingTier(string ipAddress)
    {
        return ipAddress == null ? PricingTier.Free : PricingTier.Paid;
    }
}
