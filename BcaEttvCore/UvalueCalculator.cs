using System;
using System.Collections.Generic;
using BcaEttvCore;

namespace BcaEttvCore
{
    public static class UvalueCalculator
    {
        // Assumes Thickness is in millimetres; converts to meters internally.
        // Uses standard internal/external surface resistances (Rsi=0.13, Rse=0.04 m²K/W).
        public static double ComputeUValue(List<EttvMaterial> materials)
        {
            if (materials == null || materials.Count == 0) return 0.0;

            const double Rsi = 0.12;
            const double Rse = 0.044;

            double rLayers = 0.0;
            foreach (var m in materials)
            {
                if (m == null) continue;
                if (m.ThermalConductivity <= 0) continue;

                double thicknessMeters = m.Thickness / 1000.0;
                rLayers += thicknessMeters / m.ThermalConductivity;
            }

            double rTotal = Rsi + rLayers + Rse;
            if (rTotal <= 0) return 0.0;

            return 1.0 / rTotal;
        }
    }
}