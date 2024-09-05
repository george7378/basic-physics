using Microsoft.Xna.Framework;

namespace PhysicsCore.Utility
{
    public class PointLight
    {
        #region Properties

        public Vector3 Position { get; set; }

        public float Power { get; set; }

        public float AmbientPower { get; set; }

        public float Attenuation { get; set; }

        public float SpecularExponent { get; set; }

        #endregion

        #region Constructors

        public PointLight(Vector3 position, float power, float ambientPower, float attenuation, float specularExponent)
        {
            Position = position;
            Power = power;
            AmbientPower = ambientPower;
            Attenuation = attenuation;
            SpecularExponent = specularExponent;
        }

        #endregion
    }
}