﻿using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Enums;

namespace Coding_Clean_Safe_REST_API_V_8_MinimalApi.Interfaces;

public interface IPricingTierService
{
    public PricingTier GetPricingTier(string ipAddress);
}

