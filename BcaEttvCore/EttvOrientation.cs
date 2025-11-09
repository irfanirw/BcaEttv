using Rhino.Geometry;
using System;

namespace BcaEttvCore
{
    public class EttvOrientation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector3d Normal { get; set; }

        public EttvOrientation()
        {
            Id = string.Empty;
            Name = string.Empty;
            Normal = Vector3d.ZAxis; // default normal pointing up
        }

        /// <summary>
        /// Assign Name and Id based on the Normal vector direction.
        /// Supports 8 cardinal directions on XY plane plus vertical (Roof/Floor).
        /// </summary>
        public void AssignOrientation()
        {
            if (Normal.Length == 0)
            {
                Id = "Unknown";
                Name = "Unknown";
                return;
            }

            // Determine primary direction based on normal vector
            var absX = Math.Abs(Normal.X);
            var absY = Math.Abs(Normal.Y);
            var absZ = Math.Abs(Normal.Z);

            // Check if surface is more vertical (roof/floor)
            if (absZ > absX && absZ > absY)
            {
                if (Normal.Z > 0)
                {
                    Id = "R";
                    Name = "Roof";
                }
                else
                {
                    Id = "F";
                    Name = "Floor";
                }
                return;
            }

            // Horizontal surface - determine cardinal direction on XY plane
            // Calculate angle from positive X axis (East)
            double angle = Math.Atan2(Normal.Y, Normal.X) * 180.0 / Math.PI;
            if (angle < 0) angle += 360;

            // 8 cardinal directions (45° each)
            if (angle >= 337.5 || angle < 22.5)
            {
                Id = "E";
                Name = "East";
            }
            else if (angle >= 22.5 && angle < 67.5)
            {
                Id = "NE";
                Name = "NorthEast";
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                Id = "N";
                Name = "North";
            }
            else if (angle >= 112.5 && angle < 157.5)
            {
                Id = "NW";
                Name = "NorthWest";
            }
            else if (angle >= 157.5 && angle < 202.5)
            {
                Id = "W";
                Name = "West";
            }
            else if (angle >= 202.5 && angle < 247.5)
            {
                Id = "SW";
                Name = "SouthWest";
            }
            else if (angle >= 247.5 && angle < 292.5)
            {
                Id = "S";
                Name = "South";
            }
            else // 292.5 to 337.5
            {
                Id = "SE";
                Name = "SouthEast";
            }
        }
    }
}