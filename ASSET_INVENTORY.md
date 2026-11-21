# TRASH RUSH 3D — Asset Inventory

> Tracks every visual and audio asset in the project.
> An asset is NOT marked complete until it exists on disk and works in Unity.

---

## Status Legend
- 🔲 Planned
- 🔨 In Progress
- ✅ Complete & Tested
- ⚠️ Needs Alternative
- ❌ Failed / Blocked

---

## 1. 3D Models

### 1.1 Player Character

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Player Body | Mesh (procedural) | C# MeshBuilder — capsule torso, limbs, head | `Assets/Art/Models/Player/` | 🔲 |
| Player Idle Animation | AnimationClip | C# procedural (subtle bob) | `Assets/Art/Models/Player/` | 🔲 |
| Player Walk Animation | AnimationClip | C# procedural (bounce cycle) | `Assets/Art/Models/Player/` | 🔲 |

### 1.2 Trash Items — Plastic

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Plastic Bottle | Mesh (procedural) | C# MeshBuilder — cylinder body, cone neck, cap | `Assets/Prefabs/Trash/` | 🔲 |
| Plastic Cup | Mesh (procedural) | C# MeshBuilder — tapered cylinder | `Assets/Prefabs/Trash/` | 🔲 |
| Plastic Container | Mesh (procedural) | C# MeshBuilder — rounded box | `Assets/Prefabs/Trash/` | 🔲 |

### 1.3 Trash Items — Paper

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Newspaper | Mesh (procedural) | C# MeshBuilder — folded flat box, slight crumple | `Assets/Prefabs/Trash/` | 🔲 |
| Cardboard Box | Mesh (procedural) | C# MeshBuilder — open-top box with flaps | `Assets/Prefabs/Trash/` | 🔲 |
| Paper Ball | Mesh (procedural) | C# MeshBuilder — crumpled sphere (displaced icosphere) | `Assets/Prefabs/Trash/` | 🔲 |

### 1.4 Trash Items — Metal

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Soda Can | Mesh (procedural) | C# MeshBuilder — cylinder with beveled top/pull tab | `Assets/Prefabs/Trash/` | 🔲 |
| Metal Food Can | Mesh (procedural) | C# MeshBuilder — cylinder with ridges, open top | `Assets/Prefabs/Trash/` | 🔲 |
| Metal Lid | Mesh (procedural) | C# MeshBuilder — flat disc with rim | `Assets/Prefabs/Trash/` | 🔲 |

### 1.5 Trash Items — Organic

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Banana Peel | Mesh (procedural) | C# MeshBuilder — curved strips from center | `Assets/Prefabs/Trash/` | 🔲 |
| Apple Core | Mesh (procedural) | C# MeshBuilder — hourglass sphere with stem | `Assets/Prefabs/Trash/` | 🔲 |
| Leaf Pile | Mesh (procedural) | C# MeshBuilder — cluster of curved quads | `Assets/Prefabs/Trash/` | 🔲 |

### 1.6 Recycling Bins

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Plastic Bin (Yellow) | Mesh (procedural) | C# MeshBuilder — bin body + lid + ♳ symbol + "PLASTIC" label | `Assets/Prefabs/RecyclingBins/` | 🔲 |
| Paper Bin (Blue) | Mesh (procedural) | C# MeshBuilder — bin body + lid + 📄 symbol + "PAPER" label | `Assets/Prefabs/RecyclingBins/` | 🔲 |
| Metal Bin (Gray) | Mesh (procedural) | C# MeshBuilder — bin body + lid + ⚙ symbol + "METAL" label | `Assets/Prefabs/RecyclingBins/` | 🔲 |
| Organic Bin (Green) | Mesh (procedural) | C# MeshBuilder — bin body + lid + 🌿 symbol + "ORGANIC" label | `Assets/Prefabs/RecyclingBins/` | 🔲 |

### 1.7 Environment — Park

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Park Ground | Mesh + Material | C# MeshBuilder — shaped terrain with grass | `Assets/Prefabs/Environment/` | 🔲 |
| Walking Paths | Mesh + Material | C# MeshBuilder — curved path strips | `Assets/Prefabs/Environment/` | 🔲 |
| Pine Tree (tall) | Mesh (procedural) | C# MeshBuilder — trunk + layered cone foliage | `Assets/Prefabs/Environment/` | 🔲 |
| Round Tree | Mesh (procedural) | C# MeshBuilder — trunk + sphere foliage | `Assets/Prefabs/Environment/` | 🔲 |
| Bush (small) | Mesh (procedural) | C# MeshBuilder — clustered displaced spheres | `Assets/Prefabs/Environment/` | 🔲 |
| Bush (large) | Mesh (procedural) | C# MeshBuilder — larger clustered spheres | `Assets/Prefabs/Environment/` | 🔲 |
| Park Bench | Mesh (procedural) | C# MeshBuilder — seat + back + legs | `Assets/Prefabs/Environment/` | 🔲 |
| Street Lamp | Mesh (procedural) | C# MeshBuilder — pole + lamp head + point light | `Assets/Prefabs/Environment/` | 🔲 |
| Fence Section | Mesh (procedural) | C# MeshBuilder — posts + rails | `Assets/Prefabs/Environment/` | 🔲 |
| Flower Patch | Mesh (procedural) | C# MeshBuilder — colored petal clusters | `Assets/Prefabs/Environment/` | 🔲 |
| Rock (small) | Mesh (procedural) | C# MeshBuilder — displaced icosphere | `Assets/Prefabs/Environment/` | 🔲 |
| Rock (large) | Mesh (procedural) | C# MeshBuilder — displaced icosphere, larger | `Assets/Prefabs/Environment/` | 🔲 |
| Recycling Station Area | Composed | Grouped bins + sign + floor marking | `Assets/Prefabs/Environment/` | 🔲 |
| Park Sign | Mesh (procedural) | C# MeshBuilder — post + board with text | `Assets/Prefabs/Environment/` | 🔲 |
| Pond | Mesh + Material | C# MeshBuilder — flat ellipse with water material | `Assets/Prefabs/Environment/` | 🔲 |

---

## 2. Materials

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| M_Grass | Material | Standard shader, flat green (#4CAF50) | `Assets/Art/Materials/` | 🔲 |
| M_Path | Material | Standard shader, sandy tan (#D2B48C) | `Assets/Art/Materials/` | 🔲 |
| M_TreeTrunk | Material | Standard shader, brown (#795548) | `Assets/Art/Materials/` | 🔲 |
| M_TreeFoliage_Dark | Material | Standard shader, dark green (#2E7D32) | `Assets/Art/Materials/` | 🔲 |
| M_TreeFoliage_Light | Material | Standard shader, light green (#66BB6A) | `Assets/Art/Materials/` | 🔲 |
| M_Bush | Material | Standard shader, medium green (#43A047) | `Assets/Art/Materials/` | 🔲 |
| M_Wood | Material | Standard shader, warm wood (#8D6E63) | `Assets/Art/Materials/` | 🔲 |
| M_Metal_Light | Material | Standard metallic, silver (#BDBDBD) | `Assets/Art/Materials/` | 🔲 |
| M_Metal_Dark | Material | Standard metallic, dark (#616161) | `Assets/Art/Materials/` | 🔲 |
| M_Plastic_Clear | Material | Standard, slight transparency (#B3E5FC) | `Assets/Art/Materials/` | 🔲 |
| M_Plastic_White | Material | Standard shader, white (#F5F5F5) | `Assets/Art/Materials/` | 🔲 |
| M_Paper | Material | Standard shader, off-white (#FFF9C4) | `Assets/Art/Materials/` | 🔲 |
| M_Cardboard | Material | Standard shader, cardboard (#BCAAA4) | `Assets/Art/Materials/` | 🔲 |
| M_Organic_Green | Material | Standard shader, olive (#689F38) | `Assets/Art/Materials/` | 🔲 |
| M_Organic_Yellow | Material | Standard shader, banana (#FFD54F) | `Assets/Art/Materials/` | 🔲 |
| M_Organic_Red | Material | Standard shader, apple red (#E53935) | `Assets/Art/Materials/` | 🔲 |
| M_Bin_Plastic | Material | Standard shader, yellow (#FDD835) | `Assets/Art/Materials/` | 🔲 |
| M_Bin_Paper | Material | Standard shader, blue (#1E88E5) | `Assets/Art/Materials/` | 🔲 |
| M_Bin_Metal | Material | Standard metallic, gray (#757575) | `Assets/Art/Materials/` | 🔲 |
| M_Bin_Organic | Material | Standard shader, green (#43A047) | `Assets/Art/Materials/` | 🔲 |
| M_Player_Body | Material | Standard shader, blue (#42A5F5) | `Assets/Art/Materials/` | 🔲 |
| M_Player_Skin | Material | Standard shader, skin (#FFCC80) | `Assets/Art/Materials/` | 🔲 |
| M_Player_Hair | Material | Standard shader, brown (#5D4037) | `Assets/Art/Materials/` | 🔲 |
| M_Water | Material | Standard transparent, animated (#4FC3F7) | `Assets/Art/Materials/` | 🔲 |
| M_Stone | Material | Standard shader, gray (#9E9E9E) | `Assets/Art/Materials/` | 🔲 |
| M_Flower_Red | Material | Standard shader, red (#E91E63) | `Assets/Art/Materials/` | 🔲 |
| M_Flower_Purple | Material | Standard shader, purple (#AB47BC) | `Assets/Art/Materials/` | 🔲 |
| M_Flower_White | Material | Standard shader, white (#FFFFFF) | `Assets/Art/Materials/` | 🔲 |
| M_Lamp_Post | Material | Standard metallic, dark iron (#424242) | `Assets/Art/Materials/` | 🔲 |
| M_Lamp_Light | Material | Standard emissive, warm (#FFECB3) | `Assets/Art/Materials/` | 🔲 |

---

## 3. Audio

| Asset | Type | Method | Location | License | Status |
|---|---|---|---|---|---|
| Background Music | .wav | C# procedural generation (melodic loop) | `Assets/Audio/Music/` | Original | 🔲 |
| Pickup SFX | .wav | C# procedural generation (short pop) | `Assets/Audio/SFX/` | Original | 🔲 |
| Correct Sort SFX | .wav | C# procedural generation (ascending chime) | `Assets/Audio/SFX/` | Original | 🔲 |
| Incorrect Sort SFX | .wav | C# procedural generation (buzzer) | `Assets/Audio/SFX/` | Original | 🔲 |
| Button Click SFX | .wav | C# procedural generation (soft click) | `Assets/Audio/SFX/` | Original | 🔲 |
| Victory Jingle | .wav | C# procedural generation (triumphant fanfare) | `Assets/Audio/SFX/` | Original | 🔲 |
| Game Over Sound | .wav | C# procedural generation (descending tone) | `Assets/Audio/SFX/` | Original | 🔲 |
| Footstep SFX | .wav | C# procedural generation (soft step) | `Assets/Audio/SFX/` | Original | 🔲 |

---

## 4. UI Assets

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Game Title Logo | TextMeshPro + styled layout | In-engine UI design | `Assets/UI/` | 🔲 |
| Bin Category Icons (×4) | Texture2D (generated) | C# procedural texture generation | `Assets/UI/` | 🔲 |
| HUD Background Panel | UI Sprite (9-slice) | C# procedural texture | `Assets/UI/` | 🔲 |
| Button Sprites | UI Sprite set | C# procedural texture | `Assets/UI/` | 🔲 |
| Interaction Prompt BG | UI Sprite | C# procedural texture | `Assets/UI/` | 🔲 |

---

## 5. Animations

| Asset | Type | Method | Location | Status |
|---|---|---|---|---|
| Player Idle | AnimationClip | C# procedural — gentle breathing/bob | `Assets/Art/Models/Player/` | 🔲 |
| Player Walk | AnimationClip | C# procedural — bounce cycle | `Assets/Art/Models/Player/` | 🔲 |
| Trash Pickup | Script-driven | Lerp animation in code | PlayerInteraction.cs | 🔲 |
| Trash Deposit | Script-driven | Lerp + scale animation in code | PlayerInteraction.cs | 🔲 |
| Bin Lid Open/Close | Script-driven | Rotation lerp on bin lid | RecyclingBin.cs | 🔲 |
| Correct Sort Particles | ParticleSystem | Unity ParticleSystem — green sparkles | `Assets/Prefabs/RecyclingBins/` | 🔲 |
| Incorrect Sort Particles | ParticleSystem | Unity ParticleSystem — red puff | `Assets/Prefabs/RecyclingBins/` | 🔲 |
| Floating Score Text | Script-driven | World-space text float + fade | UIManager.cs | 🔲 |

---

*This inventory will be updated as each asset is created and verified.*
