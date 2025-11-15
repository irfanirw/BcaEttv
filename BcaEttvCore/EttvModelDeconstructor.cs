using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BcaEttvCore
{
    public static class EttvModelDeconstructor
    {
        /// <summary>
        /// Deconstruct an EttvModel into a formatted text summary.
        /// Includes project info, orientation groups, surfaces, constructions, and materials.
        /// </summary>
        /// <param name="model">The EttvModel to deconstruct</param>
        /// <returns>Formatted text summary</returns>
        public static string DeconstructToText(EttvModel model)
        {
            if (model == null)
                return "No EttvModel provided.";

            var sb = new StringBuilder();

            // Project header
            sb.AppendLine("═══════════════════════════════════════════════════");
            sb.AppendLine($"Project: {model.ProjectName}");
            sb.AppendLine($"Version: {model.Version}");
            sb.AppendLine($"Total Surfaces: {model.Surfaces?.Count ?? 0}");
            sb.AppendLine("═══════════════════════════════════════════════════");
            sb.AppendLine();

            var surfaces = model.Surfaces ?? new List<EttvSurface>();

            if (surfaces.Count > 0)
            {
                sb.AppendLine("SURFACES (ungrouped):");
                sb.AppendLine("───────────────────────────────────────────────────");

                var grouped = surfaces.GroupBy(s => s?.Orientation?.Name ?? "Unknown");

                foreach (var group in grouped)
                {
                    sb.AppendLine($"\n{group.Key.ToUpper()} ({group.Count()} surfaces)");

                    foreach (var surface in group)
                    {
                        AppendSurfaceDetails(sb, surface, "  ");
                    }
                }
            }

            // Computation result
            if (model.ComputationResult != null)
            {
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════");
                sb.AppendLine("ETTV COMPUTATION RESULT");
                sb.AppendLine("═══════════════════════════════════════════════════");
                sb.AppendLine($"ETTV Value: {model.ComputationResult.EttvValue:F2} W/m²");
                sb.AppendLine($"Limit: {model.ComputationResult.Limit:F2} W/m²");
                sb.AppendLine($"Pass: {(model.ComputationResult.Pass == true ? "✓ YES" : "✗ NO")}");
                sb.AppendLine($"Climate: {model.ComputationResult.Climate}");

                if (!string.IsNullOrWhiteSpace(model.ComputationResult.ComputationDate))
                    sb.AppendLine($"Date: {model.ComputationResult.ComputationDate}");

                if (!string.IsNullOrWhiteSpace(model.ComputationResult.Notes))
                    sb.AppendLine($"Notes: {model.ComputationResult.Notes}");

                // Orientation breakdown
                if (model.ComputationResult.OrientationBreakdown?.Count > 0)
                {
                    sb.AppendLine("\nOrientation Breakdown:");
                    foreach (var kvp in model.ComputationResult.OrientationBreakdown.OrderByDescending(x => x.Value))
                        sb.AppendLine($"  {kvp.Key}: {kvp.Value:F2} W/m²");
                }

                // Component breakdown
                if (model.ComputationResult.ComponentBreakdown?.Count > 0)
                {
                    sb.AppendLine("\nComponent Breakdown:");
                    foreach (var kvp in model.ComputationResult.ComponentBreakdown.OrderByDescending(x => x.Value))
                        sb.AppendLine($"  {kvp.Key}: {kvp.Value:F2} W/m²");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get list of unique constructions from the model
        /// </summary>
        public static List<EttvConstruction> GetUniqueConstructions(EttvModel model)
        {
            if (model?.Surfaces == null)
                return new List<EttvConstruction>();

            var uniqueConstructions = new Dictionary<string, EttvConstruction>();

            foreach (var surface in model.Surfaces)
            {
                if (surface?.Construction == null)
                    continue;

                var id = surface.Construction.Id?.ToString() ?? Guid.NewGuid().ToString();

                if (!uniqueConstructions.ContainsKey(id))
                    uniqueConstructions[id] = surface.Construction;
            }

            return uniqueConstructions.Values.ToList();
        }

        /// <summary>
        /// Get list of all surfaces from the model
        /// </summary>
        public static List<EttvSurface> GetSurfaces(EttvModel model)
        {
            return model?.Surfaces ?? new List<EttvSurface>();
        }

        private static void AppendSurfaceDetails(StringBuilder sb, EttvSurface surface, string indent)
        {
            if (surface == null)
                return;

            sb.AppendLine($"{indent}├─ Surface: {surface.Name ?? $"ID:{surface.Id}"}");
            sb.AppendLine($"{indent}│  Type: {surface.Type ?? "Unknown"}");

            if (surface.Orientation != null)
                sb.AppendLine($"{indent}│  Orientation: {surface.Orientation.Name}");

            if (surface.Construction != null)
            {
                sb.AppendLine($"{indent}│  └─ Construction: {surface.Construction.Name ?? surface.Construction.Id?.ToString() ?? "Unnamed"}");

                if (surface.Construction.Uvalue > 0)
                    sb.AppendLine($"{indent}│     U-value: {surface.Construction.Uvalue:F3} W/m²·K");

                // Fenestration-specific
                if (surface.Construction is EttvFenestrationConstruction fen)
                {
                    sb.AppendLine($"{indent}│     Sc1: {fen.Sc1:F3}");
                    sb.AppendLine($"{indent}│     Sc2: {fen.Sc2:F3}");
                    sb.AppendLine($"{indent}│     ScTotal: {fen.ScTotal:F3}");
                }

                // Materials
                if (surface.Construction.EttvMaterials != null && surface.Construction.EttvMaterials.Count > 0)
                {
                    sb.AppendLine($"{indent}│     Materials ({surface.Construction.EttvMaterials.Count}):");

                    for (int i = 0; i < surface.Construction.EttvMaterials.Count; i++)
                    {
                        var mat = surface.Construction.EttvMaterials[i];
                        var prefix = i == surface.Construction.EttvMaterials.Count - 1 ? "└─" : "├─";

                        sb.AppendLine($"{indent}│        {prefix} {mat?.Name ?? "Unnamed"}");

                        if (mat != null)
                        {
                            var subIndent = i == surface.Construction.EttvMaterials.Count - 1 ? "   " : "│  ";
                            var rValue = mat.ThermalConductivity > 0 ? mat.Thickness / mat.ThermalConductivity : 0;
                            sb.AppendLine($"{indent}│        {subIndent} k: {mat.ThermalConductivity:F3} W/m·K");
                            sb.AppendLine($"{indent}│        {subIndent} Thickness: {mat.Thickness:F3} m");
                            sb.AppendLine($"{indent}│        {subIndent} R: {rValue:F3} m²·K/W");
                        }
                    }
                }
            }

            sb.AppendLine();
        }
    }
}
