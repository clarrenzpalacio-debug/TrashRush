using System.Collections.Generic;
using UnityEngine;

namespace TrashRush.Art
{
    public static class CharacterPartMeshBuilder
    {
        public static Mesh CreateHeadMesh()
        {
            Mesh head = LowPolyMeshBuilder.CreateIcosphere(0.24f, 1, 0.01f);
            Mesh cap = LowPolyMeshBuilder.CreateBox(new Vector3(0.28f, 0.05f, 0.22f));
            Mesh visor = LowPolyMeshBuilder.CreateBox(new Vector3(0.26f, 0.03f, 0.14f));

            Matrix4x4 hMat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Matrix4x4 cMat = Matrix4x4.TRS(new Vector3(0, 0.14f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 vMat = Matrix4x4.TRS(new Vector3(0, 0.12f, 0.15f), Quaternion.Euler(10, 0, 0), Vector3.one);

            return LowPolyMeshBuilder.CombineMeshes((head, hMat), (cap, cMat), (visor, vMat));
        }

        public static Mesh CreateTorsoMesh()
        {
            Mesh body = LowPolyMeshBuilder.CreateBox(new Vector3(0.44f, 0.58f, 0.26f));
            Mesh pack = LowPolyMeshBuilder.CreateBox(new Vector3(0.34f, 0.44f, 0.16f));

            Matrix4x4 bMat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Matrix4x4 pMat = Matrix4x4.TRS(new Vector3(0, 0, -0.18f), Quaternion.identity, Vector3.one);

            return LowPolyMeshBuilder.CombineMeshes((body, bMat), (pack, pMat));
        }

        public static Mesh CreateLimbMesh(float width, float height, float depth)
        {
            // Pivot at top of limb for natural shoulder/hip rotation
            Mesh limb = LowPolyMeshBuilder.CreateBox(new Vector3(width, height, depth));
            Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, -height * 0.5f, 0), Quaternion.identity, Vector3.one);
            return LowPolyMeshBuilder.CombineMeshes((limb, mat));
        }

        public static Mesh CreateBootMesh()
        {
            Mesh leg = LowPolyMeshBuilder.CreateBox(new Vector3(0.15f, 0.46f, 0.16f));
            Mesh boot = LowPolyMeshBuilder.CreateBox(new Vector3(0.17f, 0.14f, 0.24f));

            // Pivot at hip
            Matrix4x4 lMat = Matrix4x4.TRS(new Vector3(0, -0.23f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, -0.44f, 0.04f), Quaternion.identity, Vector3.one);

            return LowPolyMeshBuilder.CombineMeshes((leg, lMat), (boot, bMat));
        }
    }
}
