# TRASH RUSH 3D — Project Plan

> **Clean the Park. Sort the Waste. Save the Environment.**

---

## 1. Project Overview

| Field | Value |
|---|---|
| **Project Name** | TRASH RUSH 3D |
| **Engine** | Unity 2021.3.40f1 LTS |
| **Language** | C# |
| **Platform** | Windows PC (Standalone) |
| **Game Type** | 3D Educational / Casual Arcade |
| **Theme** | Circular Economy & Recycling |
| **Purpose** | IT316 University Final Project |

## 2. Game Concept

The player assumes the role of an environmental cleaner assigned to restore a polluted public park. They must explore the park, collect scattered garbage, correctly identify its waste category (Plastic, Paper, Metal, Organic), and deposit it in the matching recycling bin — all before time expires.

## 3. Development Stages

### Stage 1: Project Inspection & Architecture ✅
- [x] Identify Unity version (2021.3.40f1 LTS)
- [x] Inspect existing packages and configuration
- [x] Establish folder organization
- [x] Design system architecture
- [x] Create project documentation
- [x] Define acceptance criteria

**Acceptance Criteria:**
- All documentation files created and complete.
- Folder structure matches the architecture plan.
- Architecture is reviewed and approved by Project Director.

---

### Stage 2: Environment & 3D Assets 🔲
- [ ] Create park terrain with grass
- [ ] Create walking paths
- [ ] Create/place trees (procedural primitives)
- [ ] Create/place bushes
- [ ] Create benches
- [ ] Create street lamps
- [ ] Create park decorations
- [ ] Create recycling station area
- [ ] Create 4 colored recycling bins (Plastic=Yellow, Paper=Blue, Metal=Gray, Organic=Green)
- [ ] Create trash models (bottles, paper, cans, organic waste)
- [ ] Create player character (low-poly)
- [ ] Configure materials with cartoon-style colors
- [ ] Configure scene lighting (warm sunlight, ambient)
- [ ] Place garbage items across the park
- [ ] Ensure clear navigation areas

**Acceptance Criteria:**
- Park is visually attractive with consistent low-poly style.
- All 4 bin types are present and distinguishable.
- At least 3 trash items per category are placed.
- Player character exists with appropriate scale.
- Scene runs without errors.
- Performance is acceptable (>30 FPS).

---

### Stage 3: Player & Camera 🔲
- [ ] Implement PlayerController.cs (WASD movement)
- [ ] Implement CameraController.cs (third-person follow camera with mouse look)
- [ ] Configure CharacterController or Rigidbody collisions
- [ ] Add smooth movement with configurable speed
- [ ] Add mouse sensitivity settings
- [ ] Test movement in all directions
- [ ] Test camera does not clip through geometry

**Acceptance Criteria:**
- Player moves smoothly with WASD.
- Camera follows the player in third-person view.
- Mouse controls camera rotation.
- Player cannot walk through solid objects.
- Movement feels responsive and natural.

---

### Stage 4: Trash Collection 🔲
- [ ] Implement PlayerInteraction.cs (E key detection, proximity check)
- [ ] Implement TrashItem.cs (category, visual state)
- [ ] Pickup: press E near trash → attach to carry position
- [ ] Carry: trash visually attached to player
- [ ] Deposit: press E near bin → transfer to bin
- [ ] One item at a time constraint
- [ ] Interaction prompt UI ("Press E to pick up" / "Press E to deposit")

**Acceptance Criteria:**
- Player can pick up trash with E when in range.
- Trash visually attaches to the player.
- Player can only carry one item.
- Player can deposit trash at bins with E.
- Interaction prompts appear/disappear correctly.
- Cannot pick up already-collected trash.

---

### Stage 5: Recycling & Scoring 🔲
- [ ] Implement RecyclingBin.cs (accepted category, validation)
- [ ] Implement ScoreManager.cs (+10 correct, -5 incorrect)
- [ ] Implement TrashManager.cs (tracking total/remaining/sorted)
- [ ] Implement TimerManager.cs (180s countdown, pause support)
- [ ] Prevent duplicate scoring
- [ ] Prevent exploitation (re-depositing same item)
- [ ] Win condition: all trash correctly sorted
- [ ] Lose condition: timer expires

**Acceptance Criteria:**
- Correct sorting awards +10 points.
- Incorrect sorting penalizes -5 points.
- Score cannot be exploited.
- Timer counts down from 180 seconds.
- Timer pauses when game is paused.
- Game detects win/loss conditions correctly.

---

### Stage 6: UI & Game Flow 🔲
- [ ] MainMenuController.cs (Title, Play, Instructions, Quit)
- [ ] Instructions screen with gameplay explanation
- [ ] HUD: Score, Timer, Trash collected/remaining, Interaction prompt, Objective
- [ ] PauseMenuController.cs (Resume, Restart, Main Menu)
- [ ] Victory screen (message, score, stats, Restart, Main Menu)
- [ ] GameOverController.cs (time expired, score, Restart, Main Menu)
- [ ] GameManager.cs (state machine: Menu → Playing → Paused → Won → Lost)
- [ ] Scene transitions

**Acceptance Criteria:**
- All screens are functional and navigable.
- HUD displays real-time game data.
- Pause freezes gameplay and timer.
- Victory/Game Over screens show correct data.
- All buttons work correctly.
- UI style is consistent and readable.

---

### Stage 7: Audio & Polish 🔲
- [ ] Implement AudioManager.cs (singleton, volume control)
- [ ] Background music (loop)
- [ ] Trash pickup SFX
- [ ] Correct sorting SFX
- [ ] Incorrect sorting SFX
- [ ] Button click SFX
- [ ] Victory jingle
- [ ] Game Over sound
- [ ] Visual feedback (particle effects for correct/incorrect sorting)
- [ ] Lighting polish
- [ ] Performance optimization

**Acceptance Criteria:**
- All audio plays at appropriate moments.
- Audio volume is adjustable.
- Visual feedback is clear and satisfying.
- No audio clipping or overlap issues.
- Game maintains consistent FPS.

---

### Stage 8: Final QA & Build 🔲
- [ ] Full gameplay test (start to finish)
- [ ] Edge case testing
- [ ] Missing reference audit
- [ ] Win/loss path verification
- [ ] Performance profiling
- [ ] Windows build (x86_64)
- [ ] Build test (run standalone)
- [ ] Final documentation update

**Acceptance Criteria:**
- Game is playable from start to finish without errors.
- All win/loss conditions work.
- No null reference exceptions.
- Build runs on Windows standalone.
- All documentation is complete and accurate.

---

## 4. Timeline Estimate

| Stage | Estimated Effort |
|---|---|
| Stage 1: Architecture | ✅ Complete |
| Stage 2: Environment | High |
| Stage 3: Player & Camera | Medium |
| Stage 4: Trash Collection | Medium |
| Stage 5: Recycling & Scoring | Medium |
| Stage 6: UI & Game Flow | High |
| Stage 7: Audio & Polish | Medium |
| Stage 8: Final QA & Build | Medium |

## 5. Key Constraints

- Unity 2021.3.40f1 LTS — use only compatible APIs.
- Built-in Render Pipeline (not URP/HDRP).
- Legacy Input System (InputManager, not Input System package).
- All models must be procedurally created with Unity primitives or simple meshes.
- No external asset store dependencies.
- Single-player only.
- Windows PC target only.
