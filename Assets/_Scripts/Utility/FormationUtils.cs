using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Utility
{
    public static class FormationUtils
    {
        public static List<Vector3> GetVFormationPositions(int count, float spacing = 1.5f)
        {
            List<Vector3> positions = new();

            for (int i = 0; i < count; i++)
            {
                int side = i % 2 == 0 ? 1 : -1; // alternate sides
                int row = i / 2;

                float x = side * row * spacing;
                float z = -row * spacing; // backward step each row
                positions.Add(new Vector3(x, 0, z));
            }

            return positions;
        }
    }
}