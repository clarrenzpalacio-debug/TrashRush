# TRASH RUSH 3D — Asset Inventory

> Tracks every visual and audio asset in the project.
> All assets documented below have been generated, imported, assembled into prefabs, and integrated directly into the tested scene (`Assets/Scenes/GameScene.unity` and `Assets/Scenes/SampleScene.unity`).

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

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Enhanced Player Character (Multi-material low-poly cleaner) | Mesh (.asset) + Prefab | Head, cap visor, jacket torso, backpack, pants limbs, boots with separate colored materials | `Assets/Art/Models/Mesh_Player.asset`<br>`Assets/Prefabs/Player/Player.prefab` | Original / Custom | ✅ Complete | Yes |
| Procedural Bob & Sway Walk Animation | Runtime Animation | Script-driven procedural bobbing, tilt & breathing cycle | `Assets/Scripts/Player/PlayerController.cs` | Original / Custom | ✅ Complete | Yes |

### 1.2 Trash Items — Plastic

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Plastic Bottle | Mesh (.asset) + Prefab | Procedural cylinder body + tapered neck + cap | `Assets/Art/Models/Mesh_PlasticBottle.asset`<br>`Assets/Prefabs/Trash/Trash_PlasticBottle.prefab` | Original / Custom | ✅ Complete | Yes |
| Plastic Cup | Mesh (.asset) + Prefab | Procedural tapered cylinder | `Assets/Art/Models/Mesh_PlasticCup.asset`<br>`Assets/Prefabs/Trash/Trash_PlasticCup.prefab` | Original / Custom | ✅ Complete | Yes |
| Plastic Container | Mesh (.asset) + Prefab | Procedural rounded food container + fitted lid | `Assets/Art/Models/Mesh_PlasticContainer.asset`<br>`Assets/Prefabs/Trash/Trash_PlasticContainer.prefab` | Original / Custom | ✅ Complete | Yes |

### 1.3 Trash Items — Paper

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Folded Newspaper | Mesh (.asset) + Prefab | Layered angled planar boxes | `Assets/Art/Models/Mesh_Newspaper.asset`<br>`Assets/Prefabs/Trash/Trash_Newspaper.prefab` | Original / Custom | ✅ Complete | Yes |
| Cardboard Box | Mesh (.asset) + Prefab | Box body with angled upper flaps | `Assets/Art/Models/Mesh_CardboardBox.asset`<br>`Assets/Prefabs/Trash/Trash_CardboardBox.prefab` | Original / Custom | ✅ Complete | Yes |
| Paper Ball | Mesh (.asset) + Prefab | Displaced crumpled icosphere | `Assets/Art/Models/Mesh_PaperBall.asset`<br>`Assets/Prefabs/Trash/Trash_PaperBall.prefab` | Original / Custom | ✅ Complete | Yes |

### 1.4 Trash Items — Metal

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Soda Can | Mesh (.asset) + Prefab | Cylinder with top rim and pull tab | `Assets/Art/Models/Mesh_SodaCan.asset`<br>`Assets/Prefabs/Trash/Trash_SodaCan.prefab` | Original / Custom | ✅ Complete | Yes |
| Metal Food Can | Mesh (.asset) + Prefab | Cylindrical can with corrugated ridges | `Assets/Art/Models/Mesh_FoodCan.asset`<br>`Assets/Prefabs/Trash/Trash_FoodCan.prefab` | Original / Custom | ✅ Complete | Yes |
| Metal Jar Lid | Mesh (.asset) + Prefab | Circular metal disc with raised outer edge | `Assets/Art/Models/Mesh_MetalLid.asset`<br>`Assets/Prefabs/Trash/Trash_MetalLid.prefab` | Original / Custom | ✅ Complete | Yes |

### 1.5 Trash Items — Organic

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Banana Peel | Mesh (.asset) + Prefab | Central stem with 3 spreading curved peels | `Assets/Art/Models/Mesh_BananaPeel.asset`<br>`Assets/Prefabs/Trash/Trash_BananaPeel.prefab` | Original / Custom | ✅ Complete | Yes |
| Apple Core | Mesh (.asset) + Prefab | Hourglass sculpted core, stem, top/bottom bulbs | `Assets/Art/Models/Mesh_AppleCore.asset`<br>`Assets/Prefabs/Trash/Trash_AppleCore.prefab` | Original / Custom | ✅ Complete | Yes |
| Leaf Pile | Mesh (.asset) + Prefab | Clustered layered organic leafy foliage | `Assets/Art/Models/Mesh_LeafPile.asset`<br>`Assets/Prefabs/Trash/Trash_LeafPile.prefab` | Original / Custom | ✅ Complete | Yes |

### 1.6 Recycling Bins

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Plastic Recycling Bin (Yellow) | Mesh (.asset) + Prefab | Hexagonal bin body + animated lid pivot + double-sided label + Particle feedback | `Assets/Prefabs/RecyclingBins/Bin_Plastic.prefab` | Original / Custom | ✅ Complete | Yes |
| Paper Recycling Bin (Blue) | Mesh (.asset) + Prefab | Hexagonal bin body + animated lid pivot + double-sided label + Particle feedback | `Assets/Prefabs/RecyclingBins/Bin_Paper.prefab` | Original / Custom | ✅ Complete | Yes |
| Metal Recycling Bin (Gray) | Mesh (.asset) + Prefab | Hexagonal bin body + animated lid pivot + double-sided label + Particle feedback | `Assets/Prefabs/RecyclingBins/Bin_Metal.prefab` | Original / Custom | ✅ Complete | Yes |
| Organic Recycling Bin (Green) | Mesh (.asset) + Prefab | Hexagonal bin body + animated lid pivot + double-sided label + Particle feedback | `Assets/Prefabs/RecyclingBins/Bin_Organic.prefab` | Original / Custom | ✅ Complete | Yes |

### 1.7 Environment — Park Props, Nature & Park Structures

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Multi-Part Pine Tree | Multi-mesh Prefab | Bark trunk + 3-tiered dark pine foliage | `Assets/Prefabs/Environment/Env_PineTree.prefab` | Original / Custom | ✅ Complete | Yes |
| Multi-Part Round Tree | Multi-mesh Prefab | Bark trunk + faceted leafy round canopy | `Assets/Prefabs/Environment/Env_RoundTree.prefab` | Original / Custom | ✅ Complete | Yes |
| Low-Poly Bush | Mesh (.asset) + Prefab | Clustered displaced spherical shrubs | `Assets/Prefabs/Environment/Env_Bush.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Bench | Mesh (.asset) + Prefab | Slatted wooden bench with backrest and 4 legs | `Assets/Prefabs/Environment/Env_ParkBench.prefab` | Original / Custom | ✅ Complete | Yes |
| Illuminated Street Lamp | Multi-part Prefab | Cast iron post + glass lantern + real Point Light | `Assets/Prefabs/Environment/Env_StreetLamp.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Boundary Fence Section | Mesh (.asset) + Prefab | Wooden support posts + horizontal rails | `Assets/Prefabs/Environment/Env_Fence.prefab` | Original / Custom | ✅ Complete | Yes |
| Low-Poly Rock | Mesh (.asset) + Prefab | Jittered faceted stone mesh | `Assets/Prefabs/Environment/Env_Rock.prefab` | Original / Custom | ✅ Complete | Yes |
| Flower Cluster (Red/Yellow/White/Purple) | Mesh (.asset) + Prefab | Stems + petal rings + colorful bloom variants | `Assets/Art/Models/Mesh_FlowerCluster.asset`<br>`Assets/Prefabs/Environment/Env_FlowerCluster.prefab` | Original / Custom | ✅ Complete | Yes |
| Tiered Park Fountain | Mesh (.asset) + Prefab | Stone basin + rim + pedestal pillar + top bowl | `Assets/Art/Models/Mesh_Fountain.asset`<br>`Assets/Prefabs/Environment/Env_Fountain.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Picnic Table | Mesh (.asset) + Prefab | Table top plank + 2 attached benches + leg braces | `Assets/Art/Models/Mesh_PicnicTable.asset`<br>`Assets/Prefabs/Environment/Env_PicnicTable.prefab` | Original / Custom | ✅ Complete | Yes |
| Decorative Small Trash Can | Mesh (.asset) + Prefab | Cast metal cylinder with beveled rim & base | `Assets/Art/Models/Mesh_SmallTrashCan.asset`<br>`Assets/Prefabs/Environment/Env_SmallTrashCan.prefab` | Original / Custom | ✅ Complete | Yes |
| Stepping Stones | Mesh (.asset) + Prefab | Flattened natural stones | `Assets/Art/Models/Mesh_SteppingStone.asset`<br>`Assets/Prefabs/Environment/Env_SteppingStone.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Gazebo | Mesh (.asset) + Prefab | 4 wooden pillars + floor platform + conical roof | `Assets/Art/Models/Mesh_Gazebo.asset`<br>`Assets/Prefabs/Environment/Env_Gazebo.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Information Sign | Mesh (.asset) + Prefab | Upright timber post with display board | `Assets/Prefabs/Environment/Env_ParkSign.prefab` | Original / Custom | ✅ Complete | Yes |
| Park Ground (70m × 70m) | Mesh + Material | Plane mesh with stylized grass and dirt border | In Scene (`Assets/Scenes/GameScene.unity`) | Original / Custom | ✅ Complete | Yes |
| Walking Paths (North/South, East/West, Diagonals, Central Plaza) | Mesh + Material | Sandy paths, concrete central plaza, and connector diagonals | In Scene (`Assets/Scenes/GameScene.unity`) | Original / Custom | ✅ Complete | Yes |
| Scenic Park Pond | Mesh + Material | Cylindrical body with water shader | In Scene (`Assets/Scenes/GameScene.unity`) | Original / Custom | ✅ Complete | Yes |
| Complete Recycling Station Area | Assembled Scene Area | Concrete pad + 4 colored bins + station sign | In Scene (`Assets/Scenes/GameScene.unity`) | Original / Custom | ✅ Complete | Yes |

---

## 2. Materials & Color Palette

| Asset Name | Shader | Color / Settings | File Location | Status | Tested in Unity |
|---|---|---|---|---|---|
| `M_Grass` | Standard | Green (#4CAF50), smoothness 0.2 | `Assets/Art/Materials/M_Grass.mat` | ✅ Complete | Yes |
| `M_Dirt` | Standard | Earthy Brown (#846647), smoothness 0.1 | `Assets/Art/Materials/M_Dirt.mat` | ✅ Complete | Yes |
| `M_Concrete` | Standard | Light Gray (#C2C2BD), smoothness 0.2 | `Assets/Art/Materials/M_Concrete.mat` | ✅ Complete | Yes |
| `M_Path` | Standard | Sandy Tan (#D2B48C), smoothness 0.2 | `Assets/Art/Materials/M_Path.mat` | ✅ Complete | Yes |
| `M_TreeTrunk` | Standard | Bark Brown (#795548), smoothness 0.2 | `Assets/Art/Materials/M_TreeTrunk.mat` | ✅ Complete | Yes |
| `M_TreeFoliage_Dark` | Standard | Deep Pine Green (#2E7D32), smoothness 0.2 | `Assets/Art/Materials/M_TreeFoliage_Dark.mat` | ✅ Complete | Yes |
| `M_TreeFoliage_Light` | Standard | Lime Green (#66BB6A), smoothness 0.2 | `Assets/Art/Materials/M_TreeFoliage_Light.mat` | ✅ Complete | Yes |
| `M_Bush` | Standard | Vibrant Shrub Green (#43A047), smoothness 0.2 | `Assets/Art/Materials/M_Bush.mat` | ✅ Complete | Yes |
| `M_Wood` | Standard | Warm Wood (#8D6E63), smoothness 0.2 | `Assets/Art/Materials/M_Wood.mat` | ✅ Complete | Yes |
| `M_Metal_Light` | Standard | Polished Aluminum (#BDBDBD), metallic 0.8 | `Assets/Art/Materials/M_Metal_Light.mat` | ✅ Complete | Yes |
| `M_Metal_Dark` | Standard | Cast Iron (#616161), metallic 0.8 | `Assets/Art/Materials/M_Metal_Dark.mat` | ✅ Complete | Yes |
| `M_Plastic_Clear` | Standard | Translucent Pale Cyan (#B3E5FC), smoothness 0.5 | `Assets/Art/Materials/M_Plastic_Clear.mat` | ✅ Complete | Yes |
| `M_Plastic_White` | Standard | Clean White Plastic (#F5F5F5), smoothness 0.3 | `Assets/Art/Materials/M_Plastic_White.mat` | ✅ Complete | Yes |
| `M_Paper` | Standard | Warm Parchment / Newsprint (#FFF9C4) | `Assets/Art/Materials/M_Paper.mat` | ✅ Complete | Yes |
| `M_Cardboard` | Standard | Kraft Brown Cardboard (#BCAAA4) | `Assets/Art/Materials/M_Cardboard.mat` | ✅ Complete | Yes |
| `M_Organic_Green` | Standard | Foliage Green (#689F38) | `Assets/Art/Materials/M_Organic_Green.mat` | ✅ Complete | Yes |
| `M_Organic_Yellow` | Standard | Banana Peel Yellow (#FFD54F) | `Assets/Art/Materials/M_Organic_Yellow.mat` | ✅ Complete | Yes |
| `M_Organic_Red` | Standard | Ripe Apple Red (#E53935) | `Assets/Art/Materials/M_Organic_Red.mat` | ✅ Complete | Yes |
| `M_Bin_Plastic` | Standard | Vibrant Yellow (#FDD835) | `Assets/Art/Materials/M_Bin_Plastic.mat` | ✅ Complete | Yes |
| `M_Bin_Paper` | Standard | Recycling Blue (#1E88E5) | `Assets/Art/Materials/M_Bin_Paper.mat` | ✅ Complete | Yes |
| `M_Bin_Metal` | Standard | Metallic Silver/Gray (#757575), metallic 0.6 | `Assets/Art/Materials/M_Bin_Metal.mat` | ✅ Complete | Yes |
| `M_Bin_Organic` | Standard | Organic Green (#43A047) | `Assets/Art/Materials/M_Bin_Organic.mat` | ✅ Complete | Yes |
| `M_Bin_Lid` | Standard | Neutral Graphite (#4D4D4D), metallic 0.6 | `Assets/Art/Materials/M_Bin_Lid.mat` | ✅ Complete | Yes |
| `M_Flower_Red` | Standard | Vibrant Red (#E81E63) | `Assets/Art/Materials/M_Flower_Red.mat` | ✅ Complete | Yes |
| `M_Flower_Yellow` | Standard | Buttercup Yellow (#FFEB3B) | `Assets/Art/Materials/M_Flower_Yellow.mat` | ✅ Complete | Yes |
| `M_Flower_White` | Standard | Pure White (#FAFAFA) | `Assets/Art/Materials/M_Flower_White.mat` | ✅ Complete | Yes |
| `M_Flower_Purple` | Standard | Lavender Purple (#AB47BC) | `Assets/Art/Materials/M_Flower_Purple.mat` | ✅ Complete | Yes |
| `M_Fountain_Stone` | Standard | Carved Sandstone (#B3ADA6) | `Assets/Art/Materials/M_Fountain_Stone.mat` | ✅ Complete | Yes |
| `M_Player_Body` | Standard | Cleaner Uniform Blue (#2979FF) | `Assets/Art/Materials/M_Player_Body.mat` | ✅ Complete | Yes |
| `M_Player_Skin` | Standard | Warm Peach Skin Tone (#FFCC80) | `Assets/Art/Materials/M_Player_Skin.mat` | ✅ Complete | Yes |
| `M_Player_Pants` | Standard | Navy Pants (#383847) | `Assets/Art/Materials/M_Player_Pants.mat` | ✅ Complete | Yes |
| `M_Player_Boots` | Standard | Leather Boots (#4D3829) | `Assets/Art/Materials/M_Player_Boots.mat` | ✅ Complete | Yes |
| `M_Player_Backpack` | Standard | Utility Pack (#99702E) | `Assets/Art/Materials/M_Player_Backpack.mat` | ✅ Complete | Yes |
| `M_Stone` | Standard | Weathered Stone Gray (#9E9E9E) | `Assets/Art/Materials/M_Stone.mat` | ✅ Complete | Yes |
| `M_Water` | Standard | Shimmering Park Water (#4FC3F7) | `Assets/Art/Materials/M_Water.mat` | ✅ Complete | Yes |
| `M_Lamp_Post` | Standard | Dark Iron (#424242), metallic 0.8 | `Assets/Art/Materials/M_Lamp_Post.mat` | ✅ Complete | Yes |
| `M_Lamp_Light` | Standard | Emissive Warm Glow (#FFECB3) | `Assets/Art/Materials/M_Lamp_Light.mat` | ✅ Complete | Yes |
| `M_Particle_Green` | Standard Unlit (Fade) | Translucent Sparkle Green | `Assets/Art/Materials/M_Particle_Green.mat` | ✅ Complete | Yes |
| `M_Particle_Red` | Standard Unlit (Fade) | Translucent Puff Red | `Assets/Art/Materials/M_Particle_Red.mat` | ✅ Complete | Yes |

---

## 3. Audio Assets

| Asset Name | Format | Duration | Generation Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|---|
| Background Music (`BGM_ParkLoop`) | 16-bit PCM .wav | 16.0s (seamless loop) | Procedural chord progression (C-G-Am-F), bass, arpeggio, ambient pads & ticks | `Assets/Audio/Music/BGM_ParkLoop.wav` | Original / Custom | ✅ Complete | Yes |
| Trash Pickup SFX (`SFX_Pickup`) | 16-bit PCM .wav | 0.25s | Rising frequency bubble-pop sweep (400Hz → 950Hz) | `Assets/Audio/SFX/SFX_Pickup.wav` | Original / Custom | ✅ Complete | Yes |
| Correct Sort SFX (`SFX_CorrectSort`) | 16-bit PCM .wav | 0.50s | High two-tone major chime (C6 → G6) | `Assets/Audio/SFX/SFX_CorrectSort.wav` | Original / Custom | ✅ Complete | Yes |
| Incorrect Sort SFX (`SFX_IncorrectSort`) | 16-bit PCM .wav | 0.45s | Low dissonant buzzer with rasp pulse | `Assets/Audio/SFX/SFX_IncorrectSort.wav` | Original / Custom | ✅ Complete | Yes |
| Button Click SFX (`SFX_ButtonClick`) | 16-bit PCM .wav | 0.08s | Crisp high-frequency exponential UI click | `Assets/Audio/SFX/SFX_ButtonClick.wav` | Original / Custom | ✅ Complete | Yes |
| Victory Jingle (`SFX_Victory`) | 16-bit PCM .wav | 1.80s | Four-note ascending major fanfare (C5-E5-G5-C6) | `Assets/Audio/SFX/SFX_Victory.wav` | Original / Custom | ✅ Complete | Yes |
| Game Over Sound (`SFX_GameOver`) | 16-bit PCM .wav | 1.60s | Four-note descending pitch bend motif | `Assets/Audio/SFX/SFX_GameOver.wav` | Original / Custom | ✅ Complete | Yes |
| Footstep SFX (`SFX_Footstep`) | 16-bit PCM .wav | 0.12s | Soft grass thud with low-frequency rustle | `Assets/Audio/SFX/SFX_Footstep.wav` | Original / Custom | ✅ Complete | Yes |

---

## 4. UI Textures & HUD Elements

| Asset Name | Asset Type | Method | File Location | License | Status | Tested in Unity |
|---|---|---|---|---|---|---|
| Plastic Category Label Texture | PNG Texture2D | Procedural bordered graphic with category markings | `Assets/Art/Textures/Tex_Bin_Plastic.png` | Original / Custom | ✅ Complete | Yes |
| Paper Category Label Texture | PNG Texture2D | Procedural bordered graphic with category markings | `Assets/Art/Textures/Tex_Bin_Paper.png` | Original / Custom | ✅ Complete | Yes |
| Metal Category Label Texture | PNG Texture2D | Procedural bordered graphic with category markings | `Assets/Art/Textures/Tex_Bin_Metal.png` | Original / Custom | ✅ Complete | Yes |
| Organic Category Label Texture | PNG Texture2D | Procedural bordered graphic with category markings | `Assets/Art/Textures/Tex_Bin_Organic.png` | Original / Custom | ✅ Complete | Yes |
| Dynamic In-Game HUD | Canvas UI | Top bar HUD (Score, Timer, Cleaned Waste Count, Dynamic Interaction Prompt) | In Scene (`Assets/Scenes/GameScene.unity`) | Built-in | ✅ Complete | Yes |
| Victory & Game Over Panels | Overlay UI | Modal overlays with score display and restart hooks | In Scene (`Assets/Scenes/GameScene.unity`) | Built-in | ✅ Complete | Yes |

---

## 5. Animations & Interactive Feedback

| Asset Name | Type | Description | Location / Driver | Status | Tested in Unity |
|---|---|---|---|---|---|
| Player Walk Bob & Tilt | Procedural | Sinusoidal vertical bounce and horizontal tilt responding to velocity | `PlayerController.cs` | ✅ Complete | Yes |
| Player Idle Breathing | Procedural | Smooth sinusoidal breathing motion when stationary | `PlayerController.cs` | ✅ Complete | Yes |
| Uncollected Trash Hover & Spin | Procedural | Continuous 360° rotation making items visually discoverable | `TrashItem.cs` | ✅ Complete | Yes |
| Bin Lid Proximity Open / Close | Procedural Lerp | Smooth lid rotation (-30°) when the cleaner approaches | `RecyclingBin.cs` | ✅ Complete | Yes |
| Correct Sort Particle Burst | ParticleSystem | Green sparkles emitting upon correct deposit | `RecyclingBin.cs` | ✅ Complete | Yes |
| Incorrect Sort Particle Burst | ParticleSystem | Red puff emitting upon wrong deposit | `RecyclingBin.cs` | ✅ Complete | Yes |
| Item Pickup & Carry Attachment | Hierarchy Transform | Immediate parenting to player hand `CarryPoint` | `PlayerInteraction.cs` | ✅ Complete | Yes |
