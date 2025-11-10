using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace BcaEttvCore
{
    public class EttvCalculator
    {
        public EttvModel Model { get; set; }
        public double EttvLimit { get; set; } = 50.0; // Default ETTV limit (W/m²)
        public string Climate { get; set; } = "Tropical"; // Default climate

        public EttvCalculator() { }

        public EttvCalculator(EttvModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Calculate ETTV for the model and populate ComputationResult.
        /// Returns the calculated ETTV value.
        /// </summary>
        public double? CalculateEttv()
        {
            if (Model == null || Model.Surfaces == null || Model.Surfaces.Count == 0)
            {
                Model.ComputationResult = new EttvComputationResult
                {
                    EttvValue = null,
                    Pass = false,
                    Limit = EttvLimit,
                    Climate = Climate,
                    Notes = "No surfaces provided for calculation."
                };
                return null;
            }

            double wallArea = 0.0;
            double windowArea = 0.0;
            double roofArea = 0.0;

            double wallContribution = 0.0;
            double windowSolarContribution = 0.0;
            double windowConductiveContribution = 0.0;
            double roofContribution = 0.0;

            var orientationBreakdown = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var componentBreakdown = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var surface in Model.Surfaces)
            {
                if (surface == null) continue;

                double area = GetSurfaceArea(surface);
                if (area <= double.Epsilon) continue;

                string orientation = surface.Orientation?.Name ?? "Unknown";
                string componentKey = GetComponentKey(surface, orientation);
                double uValue = surface.Construction?.Uvalue ?? 0.0;
                double contribution = 0.0;

                if (IsFenestration(surface))
                {
                    windowArea += area;

                    double sc = GetScTotal(surface.Construction as EttvFenestrationConstruction);
                    double cf = GetCoolingFactor(orientation);
                    double solarGain = cf * area * sc;
                    double conductiveGain = uValue * area;

                    windowSolarContribution += solarGain;
                    windowConductiveContribution += conductiveGain;
                    contribution = solarGain + conductiveGain;
                }
                else if (IsRoof(orientation))
                {
                    roofArea += area;
                    contribution = uValue * area;
                    roofContribution += contribution;
                }
                else
                {
                    wallArea += area;
                    contribution = uValue * area;
                    wallContribution += contribution;
                }

                AddContribution(orientationBreakdown, orientation, contribution);
                AddContribution(componentBreakdown, componentKey, contribution);
            }

            double totalArea = wallArea + windowArea + roofArea;
            double numerator = wallContribution + windowSolarContribution + windowConductiveContribution + roofContribution;
            double ettvValue = totalArea > 0 ? numerator / totalArea : 0.0;
            bool pass = ettvValue <= EttvLimit;

            Model.ComputationResult = new EttvComputationResult(ettvValue, pass, EttvLimit)
            {
                Climate = Climate,
                OrientationBreakdown = orientationBreakdown,
                ComponentBreakdown = componentBreakdown,
                Notes = pass ? "ETTV calculation passed." : $"ETTV exceeds limit by {ettvValue - EttvLimit:F2} W/m²"
            };

            return ettvValue;
        }

        private static string GetComponentKey(EttvSurface surface, string orientation)
        {
            if (IsRoof(orientation))
                return "Roof";

            return surface?.Type ?? "Unknown";
        }

        private static bool IsRoof(string orientation) =>
            string.Equals(orientation, "Roof", StringComparison.OrdinalIgnoreCase);

        private static bool IsFenestration(EttvSurface surface) =>
            string.Equals(surface?.Type, "Fenestration", StringComparison.OrdinalIgnoreCase) ||
            surface?.Construction is EttvFenestrationConstruction;

        private static void AddContribution(Dictionary<string, double> map, string key, double value)
        {
            if (value == 0.0) return;

            key ??= "Unknown";
            if (!map.ContainsKey(key))
                map[key] = 0.0;
            map[key] += value;
        }

        private static double GetScTotal(EttvFenestrationConstruction fen)
        {
            if (fen == null) return 1.0;

            if (fen.ScTotal > 0.0)
                return fen.ScTotal;

            double sc1 = fen.Sc1 > 0.0 ? fen.Sc1 : 1.0;
            double sc2 = fen.Sc2 > 0.0 ? fen.Sc2 : 1.0;
            double sc = sc1 * sc2;
            return sc > 0.0 ? sc : 1.0;
        }

        private static double GetCoolingFactor(string orientation)
        {
            if (string.IsNullOrEmpty(orientation))
                return 211.0;

            if (CoolingFactors.TryGetValue(orientation, out var value))
                return value;

            return 211.0;
        }

        private static double GetSurfaceArea(EttvSurface surface)
        {
            if (surface?.Geometry is not Mesh mesh) return 0.0;
            if (!mesh.IsValid || mesh.Faces.Count == 0) return 0.0;

            var amp = AreaMassProperties.Compute(mesh);
            if (amp != null)
                return amp.Area;

            double area = 0.0;
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var face = mesh.Faces[i];
                var a = mesh.Vertices[face.A];
                var b = mesh.Vertices[face.B];
                var c = mesh.Vertices[face.C];
                area += TriangleArea(a, b, c);

                if (face.IsQuad)
                {
                    var d = mesh.Vertices[face.D];
                    area += TriangleArea(a, c, d);
                }
            }

            return area;
        }

        private static double TriangleArea(Point3f a, Point3f b, Point3f c)
        {
            var v1 = new Vector3d(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            var v2 = new Vector3d(c.X - a.X, c.Y - a.Y, c.Z - a.Z);
            return 0.5 * Vector3d.CrossProduct(v1, v2).Length;
        }

        private static readonly Dictionary<string, double> CoolingFactors = new(StringComparer.OrdinalIgnoreCase)
        {
            ["North"] = 143.0,
            ["NorthEast"] = 169.0,
            ["East"] = 193.0,
            ["SouthEast"] = 211.0,
            ["South"] = 193.0,
            ["SouthWest"] = 211.0,
            ["West"] = 193.0,
            ["NorthWest"] = 169.0,
            ["Roof"] = 0.0,
            ["Floor"] = 0.0,
            ["Unknown"] = 211.0
        };
    }
}
