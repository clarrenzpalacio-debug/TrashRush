using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrashRush.Art
{
    public static class LowPolyMeshBuilder
    {
        // ------------------ Primitive Helpers ------------------

        public static Mesh CreateCylinder(float radiusBottom, float radiusTop, float height, int segments, bool flatShaded = true)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float halfH = height * 0.5f;

            // Side faces
            for (int i = 0; i < segments; i++)
            {
                float a1 = (float)i / segments * Mathf.PI * 2f;
                float a2 = (float)(i + 1) / segments * Mathf.PI * 2f;

                Vector3 b1 = new Vector3(Mathf.Cos(a1) * radiusBottom, -halfH, Mathf.Sin(a1) * radiusBottom);
                Vector3 b2 = new Vector3(Mathf.Cos(a2) * radiusBottom, -halfH, Mathf.Sin(a2) * radiusBottom);
                Vector3 t1 = new Vector3(Mathf.Cos(a1) * radiusTop, halfH, Mathf.Sin(a1) * radiusTop);
                Vector3 t2 = new Vector3(Mathf.Cos(a2) * radiusTop, halfH, Mathf.Sin(a2) * radiusTop);

                AddQuad(verts, tris, uvs, b1, b2, t2, t1);
            }

            // Bottom cap
            Vector3 bottomCenter = new Vector3(0, -halfH, 0);
            for (int i = 0; i < segments; i++)
            {
                float a1 = (float)i / segments * Mathf.PI * 2f;
                float a2 = (float)(i + 1) / segments * Mathf.PI * 2f;
                Vector3 b1 = new Vector3(Mathf.Cos(a1) * radiusBottom, -halfH, Mathf.Sin(a1) * radiusBottom);
                Vector3 b2 = new Vector3(Mathf.Cos(a2) * radiusBottom, -halfH, Mathf.Sin(a2) * radiusBottom);
                AddTriangle(verts, tris, uvs, bottomCenter, b2, b1);
            }

            // Top cap
            Vector3 topCenter = new Vector3(0, halfH, 0);
            for (int i = 0; i < segments; i++)
            {
                float a1 = (float)i / segments * Mathf.PI * 2f;
                float a2 = (float)(i + 1) / segments * Mathf.PI * 2f;
                Vector3 t1 = new Vector3(Mathf.Cos(a1) * radiusTop, halfH, Mathf.Sin(a1) * radiusTop);
                Vector3 t2 = new Vector3(Mathf.Cos(a2) * radiusTop, halfH, Mathf.Sin(a2) * radiusTop);
                AddTriangle(verts, tris, uvs, topCenter, t1, t2);
            }

            Mesh mesh = new Mesh();
            mesh.name = "LowPolyCylinder";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateBox(Vector3 size)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            Vector3 h = size * 0.5f;

            // Front
            AddQuad(verts, tris, uvs, new Vector3(-h.x, -h.y, h.z), new Vector3(h.x, -h.y, h.z), new Vector3(h.x, h.y, h.z), new Vector3(-h.x, h.y, h.z));
            // Back
            AddQuad(verts, tris, uvs, new Vector3(h.x, -h.y, -h.z), new Vector3(-h.x, -h.y, -h.z), new Vector3(-h.x, h.y, -h.z), new Vector3(h.x, h.y, -h.z));
            // Top
            AddQuad(verts, tris, uvs, new Vector3(-h.x, h.y, h.z), new Vector3(h.x, h.y, h.z), new Vector3(h.x, h.y, -h.z), new Vector3(-h.x, h.y, -h.z));
            // Bottom
            AddQuad(verts, tris, uvs, new Vector3(-h.x, -h.y, -h.z), new Vector3(h.x, -h.y, -h.z), new Vector3(h.x, -h.y, h.z), new Vector3(-h.x, -h.y, h.z));
            // Left
            AddQuad(verts, tris, uvs, new Vector3(-h.x, -h.y, -h.z), new Vector3(-h.x, -h.y, h.z), new Vector3(-h.x, h.y, h.z), new Vector3(-h.x, h.y, -h.z));
            // Right
            AddQuad(verts, tris, uvs, new Vector3(h.x, -h.y, h.z), new Vector3(h.x, -h.y, -h.z), new Vector3(h.x, h.y, -h.z), new Vector3(h.x, h.y, h.z));

            Mesh mesh = new Mesh();
            mesh.name = "LowPolyBox";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh CreateIcosphere(float radius, int subdivisions = 1, float jitter = 0f)
        {
            // Low poly faceted sphere
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

            List<Vector3> baseVerts = new List<Vector3>()
            {
                new Vector3(-1,  t,  0).normalized * radius,
                new Vector3( 1,  t,  0).normalized * radius,
                new Vector3(-1, -t,  0).normalized * radius,
                new Vector3( 1, -t,  0).normalized * radius,
                new Vector3( 0, -1,  t).normalized * radius,
                new Vector3( 0,  1,  t).normalized * radius,
                new Vector3( 0, -1, -t).normalized * radius,
                new Vector3( 0,  1, -t).normalized * radius,
                new Vector3( t,  0, -1).normalized * radius,
                new Vector3( t,  0,  1).normalized * radius,
                new Vector3(-t,  0, -1).normalized * radius,
                new Vector3(-t,  0,  1).normalized * radius
            };

            int[] baseTris = new int[]
            {
                0, 11, 5,   0, 5, 1,    0, 1, 7,    0, 7, 10,   0, 10, 11,
                1, 5, 9,    5, 11, 4,   11, 10, 2,  10, 7, 6,   7, 1, 8,
                3, 9, 4,    3, 4, 2,    3, 2, 6,    3, 6, 8,    3, 8, 9,
                4, 9, 5,    2, 4, 11,   6, 2, 10,   8, 6, 7,    9, 8, 1
            };

            List<Vector3> triVerts = new List<Vector3>();
            for (int i = 0; i < baseTris.Length; i += 3)
            {
                Vector3 v1 = baseVerts[baseTris[i]];
                Vector3 v2 = baseVerts[baseTris[i + 1]];
                Vector3 v3 = baseVerts[baseTris[i + 2]];

                if (subdivisions == 0)
                {
                    triVerts.Add(v1);
                    triVerts.Add(v2);
                    triVerts.Add(v3);
                }
                else
                {
                    // Subdivide once
                    Vector3 m1 = ((v1 + v2) * 0.5f).normalized * radius;
                    Vector3 m2 = ((v2 + v3) * 0.5f).normalized * radius;
                    Vector3 m3 = ((v3 + v1) * 0.5f).normalized * radius;

                    triVerts.Add(v1); triVerts.Add(m1); triVerts.Add(m3);
                    triVerts.Add(v2); triVerts.Add(m2); triVerts.Add(m1);
                    triVerts.Add(v3); triVerts.Add(m3); triVerts.Add(m2);
                    triVerts.Add(m1); triVerts.Add(m2); triVerts.Add(m3);
                }
            }

            // Faceted vertices for low-poly flat-shaded aesthetic
            System.Random rnd = new System.Random(42);
            for (int i = 0; i < triVerts.Count; i += 3)
            {
                Vector3 v1 = triVerts[i];
                Vector3 v2 = triVerts[i + 1];
                Vector3 v3 = triVerts[i + 2];

                if (jitter > 0f)
                {
                    float j = (float)(rnd.NextDouble() * 2.0 - 1.0) * jitter;
                    v1 += v1.normalized * j;
                    v2 += v2.normalized * j;
                    v3 += v3.normalized * j;
                }

                int baseIdx = verts.Count;
                verts.Add(v1);
                verts.Add(v2);
                verts.Add(v3);
                tris.Add(baseIdx);
                tris.Add(baseIdx + 1);
                tris.Add(baseIdx + 2);
            }

            Mesh mesh = new Mesh();
            mesh.name = "LowPolyIcosphere";
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static void AddTriangle(List<Vector3> verts, List<int> tris, List<Vector2> uvs, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int idx = verts.Count;
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);
            tris.Add(idx);
            tris.Add(idx + 1);
            tris.Add(idx + 2);
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0.5f, 1));
        }

        public static void AddQuad(List<Vector3> verts, List<int> tris, List<Vector2> uvs, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 v4)
        {
            int idx = verts.Count;
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);
            verts.Add(v4);

            tris.Add(idx);
            tris.Add(idx + 1);
            tris.Add(idx + 2);

            tris.Add(idx);
            tris.Add(idx + 2);
            tris.Add(idx + 3);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        // ------------------ Compound Low-Poly Generators ------------------

        public static Mesh CombineMeshes(params (Mesh mesh, Matrix4x4 matrix)[] parts)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            foreach (var part in parts)
            {
                int baseIdx = verts.Count;
                Vector3[] srcVerts = part.mesh.vertices;
                int[] srcTris = part.mesh.triangles;
                Vector2[] srcUvs = part.mesh.uv;

                for (int i = 0; i < srcVerts.Length; i++)
                {
                    verts.Add(part.matrix.MultiplyPoint3x4(srcVerts[i]));
                    if (srcUvs != null && srcUvs.Length > i)
                        uvs.Add(srcUvs[i]);
                    else
                        uvs.Add(Vector2.zero);
                }

                for (int i = 0; i < srcTris.Length; i++)
                {
                    tris.Add(baseIdx + srcTris[i]);
                }
            }

            Mesh combined = new Mesh();
            combined.SetVertices(verts);
            combined.SetTriangles(tris, 0);
            combined.SetUVs(0, uvs);
            combined.RecalculateNormals();
            combined.RecalculateBounds();
            return combined;
        }

        // 1. Plastic Bottle
        public static Mesh CreatePlasticBottleMesh()
        {
            // Body (cylinder), neck (tapered cylinder), cap (small cylinder)
            Mesh body = CreateCylinder(0.12f, 0.12f, 0.35f, 8);
            Mesh neck = CreateCylinder(0.12f, 0.05f, 0.10f, 8);
            Mesh cap = CreateCylinder(0.055f, 0.055f, 0.06f, 8);

            Matrix4x4 bodyMat = Matrix4x4.TRS(new Vector3(0, 0.175f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 neckMat = Matrix4x4.TRS(new Vector3(0, 0.40f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 capMat = Matrix4x4.TRS(new Vector3(0, 0.48f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((body, bodyMat), (neck, neckMat), (cap, capMat));
            combined.name = "PlasticBottle";
            return combined;
        }

        // 2. Plastic Cup
        public static Mesh CreatePlasticCupMesh()
        {
            Mesh cup = CreateCylinder(0.09f, 0.14f, 0.3f, 8);
            Matrix4x4 mat = Matrix4x4.TRS(new Vector3(0, 0.15f, 0), Quaternion.identity, Vector3.one);
            Mesh combined = CombineMeshes((cup, mat));
            combined.name = "PlasticCup";
            return combined;
        }

        // 3. Plastic Container
        public static Mesh CreatePlasticContainerMesh()
        {
            Mesh box = CreateBox(new Vector3(0.35f, 0.18f, 0.25f));
            Mesh lid = CreateBox(new Vector3(0.37f, 0.03f, 0.27f));
            Matrix4x4 boxMat = Matrix4x4.TRS(new Vector3(0, 0.09f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 lidMat = Matrix4x4.TRS(new Vector3(0, 0.19f, 0), Quaternion.identity, Vector3.one);
            Mesh combined = CombineMeshes((box, boxMat), (lid, lidMat));
            combined.name = "PlasticContainer";
            return combined;
        }

        // 4. Newspaper
        public static Mesh CreateNewspaperMesh()
        {
            Mesh sheet1 = CreateBox(new Vector3(0.32f, 0.04f, 0.24f));
            Mesh sheet2 = CreateBox(new Vector3(0.30f, 0.03f, 0.22f));
            Matrix4x4 m1 = Matrix4x4.TRS(new Vector3(0, 0.02f, 0), Quaternion.Euler(0, 5, 0), Vector3.one);
            Matrix4x4 m2 = Matrix4x4.TRS(new Vector3(0, 0.05f, 0), Quaternion.Euler(0, -6, 0), Vector3.one);
            Mesh combined = CombineMeshes((sheet1, m1), (sheet2, m2));
            combined.name = "Newspaper";
            return combined;
        }

        // 5. Cardboard Box
        public static Mesh CreateCardboardBoxMesh()
        {
            // Box base + angled flaps
            Mesh body = CreateBox(new Vector3(0.32f, 0.26f, 0.32f));
            Mesh flap1 = CreateBox(new Vector3(0.32f, 0.02f, 0.10f));
            Mesh flap2 = CreateBox(new Vector3(0.32f, 0.02f, 0.10f));

            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.13f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 f1Mat = Matrix4x4.TRS(new Vector3(0, 0.28f, 0.13f), Quaternion.Euler(25, 0, 0), Vector3.one);
            Matrix4x4 f2Mat = Matrix4x4.TRS(new Vector3(0, 0.28f, -0.13f), Quaternion.Euler(-25, 0, 0), Vector3.one);

            Mesh combined = CombineMeshes((body, bMat), (flap1, f1Mat), (flap2, f2Mat));
            combined.name = "CardboardBox";
            return combined;
        }

        // 6. Paper Ball
        public static Mesh CreatePaperBallMesh()
        {
            Mesh ball = CreateIcosphere(0.12f, 1, 0.035f);
            Matrix4x4 m = Matrix4x4.TRS(new Vector3(0, 0.12f, 0), Quaternion.identity, Vector3.one);
            Mesh combined = CombineMeshes((ball, m));
            combined.name = "PaperBall";
            return combined;
        }

        // 7. Soda Can
        public static Mesh CreateSodaCanMesh()
        {
            Mesh body = CreateCylinder(0.09f, 0.09f, 0.24f, 8);
            Mesh rimTop = CreateCylinder(0.092f, 0.08f, 0.03f, 8);
            Mesh tab = CreateBox(new Vector3(0.03f, 0.005f, 0.06f));

            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.12f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 rMat = Matrix4x4.TRS(new Vector3(0, 0.25f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 tMat = Matrix4x4.TRS(new Vector3(0, 0.266f, 0.02f), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((body, bMat), (rimTop, rMat), (tab, tMat));
            combined.name = "SodaCan";
            return combined;
        }

        // 8. Metal Food Can
        public static Mesh CreateFoodCanMesh()
        {
            Mesh body = CreateCylinder(0.11f, 0.11f, 0.18f, 8);
            Mesh ridge1 = CreateCylinder(0.115f, 0.115f, 0.02f, 8);
            Mesh ridge2 = CreateCylinder(0.115f, 0.115f, 0.02f, 8);

            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.09f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 r1Mat = Matrix4x4.TRS(new Vector3(0, 0.05f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 r2Mat = Matrix4x4.TRS(new Vector3(0, 0.13f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((body, bMat), (ridge1, r1Mat), (ridge2, r2Mat));
            combined.name = "MetalFoodCan";
            return combined;
        }

        // 9. Metal Lid
        public static Mesh CreateMetalLidMesh()
        {
            Mesh disc = CreateCylinder(0.14f, 0.14f, 0.02f, 8);
            Mesh rim = CreateCylinder(0.15f, 0.14f, 0.015f, 8);
            Matrix4x4 dMat = Matrix4x4.TRS(new Vector3(0, 0.01f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 rMat = Matrix4x4.TRS(new Vector3(0, 0.02f, 0), Quaternion.identity, Vector3.one);
            Mesh combined = CombineMeshes((disc, dMat), (rim, rMat));
            combined.name = "MetalLid";
            return combined;
        }

        // 10. Banana Peel
        public static Mesh CreateBananaPeelMesh()
        {
            Mesh stem = CreateCylinder(0.025f, 0.015f, 0.07f, 6);
            Mesh peel1 = CreateBox(new Vector3(0.05f, 0.015f, 0.22f));
            Mesh peel2 = CreateBox(new Vector3(0.05f, 0.015f, 0.22f));
            Mesh peel3 = CreateBox(new Vector3(0.05f, 0.015f, 0.22f));

            Matrix4x4 sMat = Matrix4x4.TRS(new Vector3(0, 0.06f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 p1Mat = Matrix4x4.TRS(new Vector3(0, 0.015f, 0.08f), Quaternion.Euler(-15, 0, 0), Vector3.one);
            Matrix4x4 p2Mat = Matrix4x4.TRS(new Vector3(0.07f, 0.015f, -0.04f), Quaternion.Euler(-10, 120, 0), Vector3.one);
            Matrix4x4 p3Mat = Matrix4x4.TRS(new Vector3(-0.07f, 0.015f, -0.04f), Quaternion.Euler(-10, -120, 0), Vector3.one);

            Mesh combined = CombineMeshes((stem, sMat), (peel1, p1Mat), (peel2, p2Mat), (peel3, p3Mat));
            combined.name = "BananaPeel";
            return combined;
        }

        // 11. Apple Core
        public static Mesh CreateAppleCoreMesh()
        {
            Mesh top = CreateIcosphere(0.08f, 1, 0.01f);
            Mesh core = CreateCylinder(0.04f, 0.04f, 0.12f, 6);
            Mesh bottom = CreateIcosphere(0.08f, 1, 0.01f);
            Mesh stem = CreateCylinder(0.012f, 0.008f, 0.08f, 4);

            Matrix4x4 tMat = Matrix4x4.TRS(new Vector3(0, 0.18f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 cMat = Matrix4x4.TRS(new Vector3(0, 0.12f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.06f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 sMat = Matrix4x4.TRS(new Vector3(0, 0.26f, 0.01f), Quaternion.Euler(15, 0, 0), Vector3.one);

            Mesh combined = CombineMeshes((top, tMat), (core, cMat), (bottom, bMat), (stem, sMat));
            combined.name = "AppleCore";
            return combined;
        }

        // 12. Leaf Pile
        public static Mesh CreateLeafPileMesh()
        {
            Mesh l1 = CreateBox(new Vector3(0.24f, 0.02f, 0.16f));
            Mesh l2 = CreateBox(new Vector3(0.22f, 0.02f, 0.18f));
            Mesh l3 = CreateBox(new Vector3(0.20f, 0.02f, 0.15f));
            Mesh l4 = CreateBox(new Vector3(0.18f, 0.02f, 0.14f));

            Matrix4x4 m1 = Matrix4x4.TRS(new Vector3(0, 0.01f, 0), Quaternion.Euler(0, 20, 0), Vector3.one);
            Matrix4x4 m2 = Matrix4x4.TRS(new Vector3(0.05f, 0.03f, 0.03f), Quaternion.Euler(10, 85, 5), Vector3.one);
            Matrix4x4 m3 = Matrix4x4.TRS(new Vector3(-0.04f, 0.05f, -0.02f), Quaternion.Euler(-8, 140, -5), Vector3.one);
            Matrix4x4 m4 = Matrix4x4.TRS(new Vector3(0.02f, 0.07f, -0.04f), Quaternion.Euler(5, -45, 10), Vector3.one);

            Mesh combined = CombineMeshes((l1, m1), (l2, m2), (l3, m3), (l4, m4));
            combined.name = "LeafPile";
            return combined;
        }

        // 13. Recycling Bin Body
        public static Mesh CreateRecyclingBinBodyMesh()
        {
            // Tapered low-poly bin body with rim
            Mesh body = CreateCylinder(0.38f, 0.46f, 1.10f, 6);
            Mesh baseRim = CreateCylinder(0.40f, 0.38f, 0.08f, 6);
            Mesh topRim = CreateCylinder(0.48f, 0.50f, 0.08f, 6);

            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.55f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 brMat = Matrix4x4.TRS(new Vector3(0, 0.04f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 trMat = Matrix4x4.TRS(new Vector3(0, 1.12f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((body, bMat), (baseRim, brMat), (topRim, trMat));
            combined.name = "RecyclingBinBody";
            return combined;
        }

        // 14. Recycling Bin Lid (with deposit opening)
        public static Mesh CreateRecyclingBinLidMesh()
        {
            // Beveled lid top + handle
            Mesh lid = CreateCylinder(0.50f, 0.36f, 0.22f, 6);
            Mesh slotHole = CreateBox(new Vector3(0.28f, 0.06f, 0.12f));
            Mesh handle = CreateBox(new Vector3(0.18f, 0.04f, 0.04f));

            Matrix4x4 lMat = Matrix4x4.TRS(new Vector3(0, 0.11f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 hMat = Matrix4x4.TRS(new Vector3(0, 0.24f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((lid, lMat), (handle, hMat));
            combined.name = "RecyclingBinLid";
            return combined;
        }

        // 15. Low-Poly Tree (Pine)
        public static Mesh CreatePineTreeMesh()
        {
            Mesh trunk = CreateCylinder(0.22f, 0.16f, 1.8f, 6);
            Mesh tier1 = CreateCylinder(1.6f, 0.2f, 1.4f, 6);
            Mesh tier2 = CreateCylinder(1.3f, 0.15f, 1.3f, 6);
            Mesh tier3 = CreateCylinder(0.9f, 0.05f, 1.2f, 6);

            Matrix4x4 tMat = Matrix4x4.TRS(new Vector3(0, 0.9f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 t1Mat = Matrix4x4.TRS(new Vector3(0, 1.8f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 t2Mat = Matrix4x4.TRS(new Vector3(0, 2.6f, 0), Quaternion.Euler(0, 30, 0), Vector3.one);
            Matrix4x4 t3Mat = Matrix4x4.TRS(new Vector3(0, 3.4f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((trunk, tMat), (tier1, t1Mat), (tier2, t2Mat), (tier3, t3Mat));
            combined.name = "PineTree";
            return combined;
        }

        // 16. Low-Poly Tree (Round/Deciduous)
        public static Mesh CreateRoundTreeMesh()
        {
            Mesh trunk = CreateCylinder(0.25f, 0.20f, 2.2f, 6);
            Mesh foliage = CreateIcosphere(1.4f, 1, 0.12f);

            Matrix4x4 tMat = Matrix4x4.TRS(new Vector3(0, 1.1f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 fMat = Matrix4x4.TRS(new Vector3(0, 2.8f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((trunk, tMat), (foliage, fMat));
            combined.name = "RoundTree";
            return combined;
        }

        // 17. Bush
        public static Mesh CreateBushMesh(float scale = 1.0f)
        {
            Mesh sphere1 = CreateIcosphere(0.6f * scale, 1, 0.05f);
            Mesh sphere2 = CreateIcosphere(0.48f * scale, 1, 0.05f);
            Mesh sphere3 = CreateIcosphere(0.52f * scale, 1, 0.05f);

            Matrix4x4 m1 = Matrix4x4.TRS(new Vector3(0, 0.5f * scale, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 m2 = Matrix4x4.TRS(new Vector3(-0.35f * scale, 0.4f * scale, 0.2f * scale), Quaternion.identity, Vector3.one);
            Matrix4x4 m3 = Matrix4x4.TRS(new Vector3(0.35f * scale, 0.42f * scale, -0.15f * scale), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((sphere1, m1), (sphere2, m2), (sphere3, m3));
            combined.name = "Bush";
            return combined;
        }

        // 18. Park Bench
        public static Mesh CreateParkBenchMesh()
        {
            // Seat plank, backrest, legs (4), armrests (2)
            Mesh seat = CreateBox(new Vector3(1.6f, 0.08f, 0.45f));
            Mesh back = CreateBox(new Vector3(1.6f, 0.35f, 0.06f));
            Mesh leg1 = CreateBox(new Vector3(0.08f, 0.45f, 0.08f));
            Mesh leg2 = CreateBox(new Vector3(0.08f, 0.45f, 0.08f));
            Mesh leg3 = CreateBox(new Vector3(0.08f, 0.45f, 0.08f));
            Mesh leg4 = CreateBox(new Vector3(0.08f, 0.45f, 0.08f));

            Matrix4x4 sMat = Matrix4x4.TRS(new Vector3(0, 0.45f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.75f, -0.20f), Quaternion.Euler(-10, 0, 0), Vector3.one);
            Matrix4x4 l1 = Matrix4x4.TRS(new Vector3(-0.68f, 0.225f, -0.16f), Quaternion.identity, Vector3.one);
            Matrix4x4 l2 = Matrix4x4.TRS(new Vector3(0.68f, 0.225f, -0.16f), Quaternion.identity, Vector3.one);
            Matrix4x4 l3 = Matrix4x4.TRS(new Vector3(-0.68f, 0.225f, 0.16f), Quaternion.identity, Vector3.one);
            Matrix4x4 l4 = Matrix4x4.TRS(new Vector3(0.68f, 0.225f, 0.16f), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((seat, sMat), (back, bMat), (leg1, l1), (leg2, l2), (leg3, l3), (leg4, l4));
            combined.name = "ParkBench";
            return combined;
        }

        // 19. Street Lamp
        public static Mesh CreateStreetLampMesh()
        {
            Mesh pole = CreateCylinder(0.08f, 0.06f, 3.2f, 6);
            Mesh baseFoot = CreateCylinder(0.20f, 0.12f, 0.25f, 6);
            Mesh topCap = CreateCylinder(0.32f, 0.10f, 0.25f, 6);
            Mesh lampGlass = CreateCylinder(0.18f, 0.26f, 0.35f, 6);

            Matrix4x4 pMat = Matrix4x4.TRS(new Vector3(0, 1.6f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 0.125f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 gMat = Matrix4x4.TRS(new Vector3(0, 3.25f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 cMat = Matrix4x4.TRS(new Vector3(0, 3.55f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((pole, pMat), (baseFoot, bMat), (lampGlass, gMat), (topCap, cMat));
            combined.name = "StreetLamp";
            return combined;
        }

        // 20. Fence Section
        public static Mesh CreateFenceSectionMesh()
        {
            Mesh post1 = CreateBox(new Vector3(0.12f, 0.9f, 0.12f));
            Mesh post2 = CreateBox(new Vector3(0.12f, 0.9f, 0.12f));
            Mesh rail1 = CreateBox(new Vector3(1.8f, 0.08f, 0.04f));
            Mesh rail2 = CreateBox(new Vector3(1.8f, 0.08f, 0.04f));

            Matrix4x4 p1 = Matrix4x4.TRS(new Vector3(-0.9f, 0.45f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 p2 = Matrix4x4.TRS(new Vector3(0.9f, 0.45f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 r1 = Matrix4x4.TRS(new Vector3(0, 0.30f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 r2 = Matrix4x4.TRS(new Vector3(0, 0.65f, 0), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((post1, p1), (post2, p2), (rail1, r1), (rail2, r2));
            combined.name = "FenceSection";
            return combined;
        }

        // 21. Rock
        public static Mesh CreateRockMesh(float scale = 1.0f)
        {
            Mesh rock = CreateIcosphere(0.5f * scale, 1, 0.12f * scale);
            Matrix4x4 m = Matrix4x4.TRS(new Vector3(0, 0.35f * scale, 0), Quaternion.identity, new Vector3(1.3f, 0.7f, 1.1f));
            Mesh combined = CombineMeshes((rock, m));
            combined.name = "Rock";
            return combined;
        }

        // 22. Park Sign
        public static Mesh CreateParkSignMesh()
        {
            Mesh post = CreateBox(new Vector3(0.10f, 1.4f, 0.10f));
            Mesh board = CreateBox(new Vector3(0.9f, 0.55f, 0.06f));

            Matrix4x4 pMat = Matrix4x4.TRS(new Vector3(0, 0.7f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bMat = Matrix4x4.TRS(new Vector3(0, 1.15f, 0.06f), Quaternion.identity, Vector3.one);

            Mesh combined = CombineMeshes((post, pMat), (board, bMat));
            combined.name = "ParkSign";
            return combined;
        }

        // 23. Player Character Mesh
        public static Mesh CreatePlayerMesh()
        {
            // Head, cap, torso (jacket), backpack/bin-bag, legs (2), boots (2), arms (2)
            Mesh head = CreateIcosphere(0.24f, 1, 0.01f);
            Mesh capVisor = CreateBox(new Vector3(0.28f, 0.04f, 0.18f));
            Mesh torso = CreateBox(new Vector3(0.44f, 0.62f, 0.28f));
            Mesh backpack = CreateBox(new Vector3(0.34f, 0.45f, 0.16f));

            Mesh legL = CreateBox(new Vector3(0.15f, 0.60f, 0.16f));
            Mesh legR = CreateBox(new Vector3(0.15f, 0.60f, 0.16f));
            Mesh bootL = CreateBox(new Vector3(0.17f, 0.14f, 0.24f));
            Mesh bootR = CreateBox(new Vector3(0.17f, 0.14f, 0.24f));

            Mesh armL = CreateBox(new Vector3(0.12f, 0.52f, 0.12f));
            Mesh armR = CreateBox(new Vector3(0.12f, 0.52f, 0.12f));

            Matrix4x4 headMat = Matrix4x4.TRS(new Vector3(0, 1.62f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 capMat = Matrix4x4.TRS(new Vector3(0, 1.76f, 0.12f), Quaternion.Euler(-10, 0, 0), Vector3.one);
            Matrix4x4 torsoMat = Matrix4x4.TRS(new Vector3(0, 1.05f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bpMat = Matrix4x4.TRS(new Vector3(0, 1.05f, -0.19f), Quaternion.identity, Vector3.one);

            Matrix4x4 legLMat = Matrix4x4.TRS(new Vector3(-0.13f, 0.44f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 legRMat = Matrix4x4.TRS(new Vector3(0.13f, 0.44f, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 bootLMat = Matrix4x4.TRS(new Vector3(-0.13f, 0.07f, 0.04f), Quaternion.identity, Vector3.one);
            Matrix4x4 bootRMat = Matrix4x4.TRS(new Vector3(0.13f, 0.07f, 0.04f), Quaternion.identity, Vector3.one);

            Matrix4x4 armLMat = Matrix4x4.TRS(new Vector3(-0.29f, 1.02f, 0), Quaternion.Euler(0, 0, 5), Vector3.one);
            Matrix4x4 armRMat = Matrix4x4.TRS(new Vector3(0.29f, 1.02f, 0), Quaternion.Euler(0, 0, -5), Vector3.one);

            Mesh combined = CombineMeshes(
                (head, headMat),
                (capVisor, capMat),
                (torso, torsoMat),
                (backpack, bpMat),
                (legL, legLMat),
                (legR, legRMat),
                (bootL, bootLMat),
                (bootR, bootRMat),
                (armL, armLMat),
                (armR, armRMat)
            );
            combined.name = "PlayerCharacter";
            return combined;
        }
    }
}
