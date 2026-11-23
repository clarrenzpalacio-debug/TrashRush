using System.IO;
using UnityEngine;

namespace TrashRush.Art
{
    public static class ProceduralTextureGenerator
    {
        public static Texture2D CreateSolidTexture(Color color, int width = 16, int height = 16)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] cols = new Color[width * height];
            for (int i = 0; i < cols.Length; i++) cols[i] = color;
            tex.SetPixels(cols);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Creates a detailed 512x256 texture for bin front/back plaques with category name,
        /// large clear icon, subtitle examples, and clean low-poly cartoon borders.
        /// </summary>
        public static Texture2D CreateEnhancedBinLabelTexture(string categoryName, Color primaryColor, string subtitleExamples)
        {
            int w = 512;
            int h = 256;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color[] cols = new Color[w * h];

            Color darkBorder = Color.Lerp(primaryColor, Color.black, 0.45f);
            Color lightBorder = Color.Lerp(primaryColor, Color.white, 0.35f);
            Color innerBg = Color.Lerp(primaryColor, Color.black, 0.2f);
            Color headerBg = Color.Lerp(primaryColor, Color.black, 0.55f);

            // Fill base frame with rounded corners & clean bevels
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Rounded corner mask
                    int rx = (x < 24) ? 24 - x : (x >= w - 24) ? x - (w - 25) : 0;
                    int ry = (y < 24) ? 24 - y : (y >= h - 24) ? y - (h - 25) : 0;
                    if (rx * rx + ry * ry > 24 * 24)
                    {
                        cols[y * w + x] = new Color(0, 0, 0, 0); // transparent outside
                        continue;
                    }

                    bool isOuterRim = (x < 10 || x >= w - 10 || y < 10 || y >= h - 10 || (rx * rx + ry * ry > 16 * 16));
                    bool isInnerRim = (x < 16 || x >= w - 16 || y < 16 || y >= h - 16);
                    bool isHeader = (y >= h - 68 && y < h - 16);
                    bool isDivider = (y >= h - 72 && y < h - 68);

                    if (isOuterRim)
                    {
                        cols[y * w + x] = darkBorder;
                    }
                    else if (isInnerRim)
                    {
                        cols[y * w + x] = lightBorder;
                    }
                    else if (isDivider)
                    {
                        cols[y * w + x] = Color.white;
                    }
                    else if (isHeader)
                    {
                        cols[y * w + x] = headerBg;
                    }
                    else
                    {
                        cols[y * w + x] = innerBg;
                    }
                }
            }
            tex.SetPixels(cols);

            // 1. Draw large Bold Category Title in Header Area
            DrawCategoryTitleText(tex, categoryName, w / 2, h - 44);

            // 2. Draw Large Central Icon Silhouette
            DrawCategoryIcon(tex, categoryName, 130, 95);

            // 3. Draw Examples Subtitle / text box on the right
            DrawCategoryInfoSection(tex, categoryName, subtitleExamples, 320, 95);

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Creates a square 256x256 billboard texture with category name & icon for floating overhead display.
        /// </summary>
        public static Texture2D CreateFloatingLabelTexture(string categoryName, Color primaryColor)
        {
            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] cols = new Color[size * size];

            Color darkBorder = Color.Lerp(primaryColor, Color.black, 0.45f);
            Color bg = Color.Lerp(primaryColor, Color.black, 0.15f);

            int cx = size / 2;
            int cy = size / 2;
            int rOuter = 118;
            int rInner = 106;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    int distSq = dx * dx + dy * dy;

                    if (distSq > rOuter * rOuter)
                    {
                        cols[y * size + x] = new Color(0, 0, 0, 0); // Transparent outside circle
                    }
                    else if (distSq > rInner * rInner)
                    {
                        cols[y * size + x] = Color.white; // White outer ring
                    }
                    else if (distSq > (rInner - 6) * (rInner - 6))
                    {
                        cols[y * size + x] = darkBorder;
                    }
                    else
                    {
                        cols[y * size + x] = bg;
                    }
                }
            }
            tex.SetPixels(cols);

            // Category Icon in Center
            DrawCategoryIcon(tex, categoryName, cx, cy + 22, 1.15f);

            // Category Name underneath
            DrawCategoryTitleText(tex, categoryName, cx, 42, 1.0f);

            tex.Apply();
            return tex;
        }

        private static void DrawCategoryTitleText(Texture2D tex, string category, int centerX, int centerY, float scale = 1.2f)
        {
            // Pixel font matrix representation for "PAPER", "PLASTIC", "METAL", "ORGANIC"
            // High visibility bold block letters
            string text = category.ToUpper();
            int letterSpacing = (int)(22 * scale);
            int startX = centerX - (text.Length * letterSpacing) / 2 + (int)(10 * scale);

            for (int i = 0; i < text.Length; i++)
            {
                DrawBlockChar(tex, text[i], startX + i * letterSpacing, centerY, scale);
            }
        }

        private static void DrawCategoryInfoSection(Texture2D tex, string category, string examples, int cx, int cy)
        {
            // Background card on right half
            int cardW = 190;
            int cardH = 130;
            Color cardBg = new Color(0, 0, 0, 0.45f);
            DrawRect(tex, cx - cardW / 2, cy - cardH / 2, cardW, cardH, cardBg);

            // Draw header "ACCEPTABLE:"
            DrawStringSimple(tex, "ACCEPTABLE:", cx - 80, cy + 38);

            // Draw 2 bullet lines of items
            if (category == "PAPER")
            {
                DrawStringSimple(tex, "- NEWSPAPERS", cx - 75, cy + 12);
                DrawStringSimple(tex, "- CARDBOARD", cx - 75, cy - 14);
                DrawStringSimple(tex, "- PAPER BALLS", cx - 75, cy - 40);
            }
            else if (category == "PLASTIC")
            {
                DrawStringSimple(tex, "- BOTTLES", cx - 75, cy + 12);
                DrawStringSimple(tex, "- CUPS", cx - 75, cy - 14);
                DrawStringSimple(tex, "- CONTAINERS", cx - 75, cy - 40);
            }
            else if (category == "METAL")
            {
                DrawStringSimple(tex, "- SODA CANS", cx - 75, cy + 12);
                DrawStringSimple(tex, "- FOOD CANS", cx - 75, cy - 14);
                DrawStringSimple(tex, "- JAR LIDS", cx - 75, cy - 40);
            }
            else if (category == "ORGANIC")
            {
                DrawStringSimple(tex, "- BANANA PEELS", cx - 75, cy + 12);
                DrawStringSimple(tex, "- APPLE CORES", cx - 75, cy - 14);
                DrawStringSimple(tex, "- LEAF PILES", cx - 75, cy - 40);
            }
        }

        private static void DrawCategoryIcon(Texture2D tex, string category, int cx, int cy, float scale = 1.0f)
        {
            Color iconColor = Color.white;
            Color darkShadow = new Color(0, 0, 0, 0.5f);

            // White badge backing
            int badgeR = (int)(55 * scale);
            DrawCircle(tex, cx, cy, badgeR, new Color(0, 0, 0, 0.4f));
            DrawCircle(tex, cx, cy, badgeR - 4, new Color(1, 1, 1, 0.15f));

            if (category == "PAPER")
            {
                // Paper Sheet with folded corner + Cardboard flap box silhouette
                int sx = (int)(28 * scale);
                int sy = (int)(38 * scale);
                // Shadow
                DrawRect(tex, cx - sx + 3, cy - sy - 3, sx * 2, sy * 2, darkShadow);
                // Sheet
                DrawRect(tex, cx - sx, cy - sy, sx * 2, sy * 2, iconColor);
                // Folded corner cutout
                DrawTriangle(tex, cx + sx - 16, cy + sy, cx + sx, cy + sy - 16, cx + sx, cy + sy, darkShadow);
                // Text lines on paper
                Color lineCol = new Color(0.2f, 0.4f, 0.8f, 1f);
                DrawRect(tex, cx - sx + 6, cy + 14, sx * 2 - 24, (int)(4 * scale), lineCol);
                DrawRect(tex, cx - sx + 6, cy + 2, sx * 2 - 12, (int)(4 * scale), lineCol);
                DrawRect(tex, cx - sx + 6, cy - 10, sx * 2 - 12, (int)(4 * scale), lineCol);
                DrawRect(tex, cx - sx + 6, cy - 22, sx * 2 - 18, (int)(4 * scale), lineCol);
            }
            else if (category == "PLASTIC")
            {
                // Plastic Water Bottle silhouette
                int wBody = (int)(24 * scale);
                int hBody = (int)(50 * scale);
                int wNeck = (int)(10 * scale);
                int hNeck = (int)(18 * scale);
                int wCap = (int)(14 * scale);
                int hCap = (int)(8 * scale);

                // Body
                DrawRect(tex, cx - wBody / 2, cy - 40, wBody, hBody, iconColor);
                // Neck taper
                DrawRect(tex, cx - wNeck / 2, cy + 10, wNeck, hNeck, iconColor);
                // Cap
                DrawRect(tex, cx - wCap / 2, cy + 28, wCap, hCap, Color.cyan);

                // Ribs on plastic bottle
                Color ribCol = new Color(0.1f, 0.6f, 0.9f, 0.8f);
                DrawRect(tex, cx - wBody / 2 + 3, cy - 26, wBody - 6, 4, ribCol);
                DrawRect(tex, cx - wBody / 2 + 3, cy - 10, wBody - 6, 4, ribCol);
                DrawRect(tex, cx - wBody / 2 + 3, cy + 4, wBody - 6, 4, ribCol);
            }
            else if (category == "METAL")
            {
                // Soda can & open top ring
                int wCan = (int)(32 * scale);
                int hCan = (int)(58 * scale);

                // Body
                DrawRect(tex, cx - wCan / 2, cy - hCan / 2, wCan, hCan, iconColor);
                // Can rims
                DrawRect(tex, cx - wCan / 2 - 2, cy + hCan / 2 - 5, wCan + 4, 6, new Color(0.85f, 0.85f, 0.85f, 1f));
                DrawRect(tex, cx - wCan / 2 - 2, cy - hCan / 2, wCan + 4, 6, new Color(0.85f, 0.85f, 0.85f, 1f));
                // Center metallic label
                Color canStripe = new Color(0.85f, 0.15f, 0.15f, 1f);
                DrawRect(tex, cx - wCan / 2 + 3, cy - 14, wCan - 6, 28, canStripe);
                // Pull tab on top
                DrawRect(tex, cx - 4, cy + hCan / 2 + 1, 8, 5, Color.white);
            }
            else if (category == "ORGANIC")
            {
                // Vibrant Apple + Leaf
                int rApple = (int)(22 * scale);
                DrawCircle(tex, cx - 12, cy - 6, rApple, iconColor);
                DrawCircle(tex, cx + 12, cy - 6, rApple, iconColor);

                // Stem
                DrawRect(tex, cx - 3, cy + 12, 6, (int)(18 * scale), new Color(0.4f, 0.25f, 0.1f, 1f));
                // Leaf
                int leafX = cx + 12;
                int leafY = cy + 22;
                DrawCircle(tex, leafX, leafY, (int)(10 * scale), new Color(0.2f, 0.85f, 0.3f, 1f));
            }
        }

        // --- Basic pixel drawing helpers ---

        private static void DrawRect(Texture2D tex, int x0, int y0, int w, int h, Color col)
        {
            for (int y = y0; y < y0 + h; y++)
            {
                for (int x = x0; x < x0 + w; x++)
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    {
                        if (col.a >= 1f) tex.SetPixel(x, y, col);
                        else
                        {
                            Color prev = tex.GetPixel(x, y);
                            tex.SetPixel(x, y, Color.Lerp(prev, col, col.a));
                        }
                    }
                }
            }
        }

        private static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color col)
        {
            int r2 = r * r;
            for (int y = cy - r; y <= cy + r; y++)
            {
                for (int x = cx - r; x <= cx + r; x++)
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    {
                        if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r2)
                        {
                            if (col.a >= 1f) tex.SetPixel(x, y, col);
                            else
                            {
                                Color prev = tex.GetPixel(x, y);
                                tex.SetPixel(x, y, Color.Lerp(prev, col, col.a));
                            }
                        }
                    }
                }
            }
        }

        private static void DrawTriangle(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color col)
        {
            int minX = Mathf.Min(x1, Mathf.Min(x2, x3));
            int maxX = Mathf.Max(x1, Mathf.Max(x2, x3));
            int minY = Mathf.Min(y1, Mathf.Min(y2, y3));
            int maxY = Mathf.Max(y1, Mathf.Max(y2, y3));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    int d1 = (x - x2) * (y1 - y2) - (x1 - x2) * (y - y2);
                    int d2 = (x - x3) * (y2 - y3) - (x2 - x3) * (y - y3);
                    int d3 = (x - x1) * (y3 - y1) - (x3 - x1) * (y - y1);
                    bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                    bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
                    if (!(hasNeg && hasPos))
                    {
                        if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                            tex.SetPixel(x, y, col);
                    }
                }
            }
        }

        private static void DrawBlockChar(Texture2D tex, char c, int cx, int cy, float scale)
        {
            int w = (int)(14 * scale);
            int h = (int)(22 * scale);
            int th = Mathf.Max(2, (int)(4 * scale)); // stroke thickness
            int x0 = cx - w / 2;
            int y0 = cy - h / 2;
            Color col = Color.white;
            Color shadow = new Color(0, 0, 0, 0.5f);

            // Draw shadow first
            DrawBlockCharGlyph(tex, c, x0 + 2, y0 - 2, w, h, th, shadow);
            DrawBlockCharGlyph(tex, c, x0, y0, w, h, th, col);
        }

        private static void DrawBlockCharGlyph(Texture2D tex, char c, int x0, int y0, int w, int h, int th, Color col)
        {
            switch (c)
            {
                case 'P':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0 + h / 2, w, th, col);
                    DrawRect(tex, x0 + w - th, y0 + h / 2, th, h / 2, col);
                    break;
                case 'A':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0 + w - th, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0 + h / 2 - th / 2, w, th, col);
                    break;
                case 'E':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0 + h / 2 - th / 2, w - 2, th, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'R':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0 + h / 2, w, th, col);
                    DrawRect(tex, x0 + w - th, y0 + h / 2, th, h / 2, col);
                    DrawRect(tex, x0 + w - th, y0, th, h / 2, col);
                    break;
                case 'L':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'S':
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0 + h / 2, th, h / 2, col);
                    DrawRect(tex, x0, y0 + h / 2 - th / 2, w, th, col);
                    DrawRect(tex, x0 + w - th, y0, th, h / 2, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'T':
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0 + w / 2 - th / 2, y0, th, h, col);
                    break;
                case 'I':
                    DrawRect(tex, x0 + w / 2 - th / 2, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'C':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'M':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0 + w - th, y0, th, h, col);
                    DrawRect(tex, x0 + th, y0 + h / 2, th, h / 2, col);
                    DrawRect(tex, x0 + w - th * 2, y0 + h / 2, th, h / 2, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    break;
                case 'O':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0 + w - th, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    break;
                case 'G':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, w, th, col);
                    DrawRect(tex, x0, y0, w, th, col);
                    DrawRect(tex, x0 + w - th, y0, th, h / 2, col);
                    DrawRect(tex, x0 + w / 2, y0 + h / 2 - th / 2, w / 2, th, col);
                    break;
                case 'N':
                    DrawRect(tex, x0, y0, th, h, col);
                    DrawRect(tex, x0 + w - th, y0, th, h, col);
                    DrawRect(tex, x0, y0 + h - th, th * 2, th, col);
                    DrawRect(tex, x0 + w / 2 - th / 2, y0 + h / 2 - th / 2, th, th, col);
                    DrawRect(tex, x0 + w - th * 2, y0, th * 2, th, col);
                    break;
                default:
                    DrawRect(tex, x0, y0, w, h, col);
                    break;
            }
        }

        private static void DrawStringSimple(Texture2D tex, string str, int startX, int startY)
        {
            int charW = 7;
            int charH = 12;
            int th = 2;
            int spacing = 9;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                int x = startX + i * spacing;
                if (c == ' ') continue;
                if (c == '-')
                {
                    DrawRect(tex, x, startY + 4, charW - 1, 2, Color.yellow);
                    continue;
                }
                DrawBlockCharGlyph(tex, c, x, startY, charW, charH, th, Color.white);
            }
        }

        public static Texture2D CreateEnhancedBinLabelTexture(TrashRush.Core.TrashCategory category)
        {
            switch (category)
            {
                case TrashRush.Core.TrashCategory.Paper:
                    return CreateEnhancedBinLabelTexture("PAPER", new Color(0.12f, 0.53f, 0.90f, 1f), "NEWSPAPERS, CARDBOARD, BOOKS");
                case TrashRush.Core.TrashCategory.Plastic:
                    return CreateEnhancedBinLabelTexture("PLASTIC", new Color(0.99f, 0.85f, 0.21f, 1f), "WATER BOTTLES, JUGS, TRAYS");
                case TrashRush.Core.TrashCategory.Metal:
                    return CreateEnhancedBinLabelTexture("METAL", new Color(0.90f, 0.22f, 0.21f, 1f), "SODA CANS, FOOD TINS, FOIL");
                case TrashRush.Core.TrashCategory.Organic:
                    return CreateEnhancedBinLabelTexture("ORGANIC", new Color(0.26f, 0.63f, 0.28f, 1f), "BANANAS, APPLES, LEAVES");
                default:
                    return CreateEnhancedBinLabelTexture(category.ToString().ToUpper(), Color.gray, "RECYCLABLE ITEMS");
            }
        }

        public static Texture2D CreateFloatingLabelTexture(TrashRush.Core.TrashCategory category)
        {
            switch (category)
            {
                case TrashRush.Core.TrashCategory.Paper:
                    return CreateFloatingLabelTexture("PAPER", new Color(0.12f, 0.53f, 0.90f, 1f));
                case TrashRush.Core.TrashCategory.Plastic:
                    return CreateFloatingLabelTexture("PLASTIC", new Color(0.99f, 0.85f, 0.21f, 1f));
                case TrashRush.Core.TrashCategory.Metal:
                    return CreateFloatingLabelTexture("METAL", new Color(0.90f, 0.22f, 0.21f, 1f));
                case TrashRush.Core.TrashCategory.Organic:
                    return CreateFloatingLabelTexture("ORGANIC", new Color(0.26f, 0.63f, 0.28f, 1f));
                default:
                    return CreateFloatingLabelTexture(category.ToString().ToUpper(), Color.gray);
            }
        }

        public static Texture2D CreateBinLabelTexture(string categoryName, Color primaryColor, Color textColor, string symbol)
        {
            return CreateEnhancedBinLabelTexture(categoryName, primaryColor, "RECYCLABLE");
        }

        public static Texture2D CreateBinLabelTexture(TrashRush.Core.TrashCategory category)
        {
            return CreateEnhancedBinLabelTexture(category);
        }

        public static void SaveTextureAsPNG(Texture2D tex, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
    }
}
