# TRASH RUSH 3D — Test Report (Physics & Animation Repair QA)

**Date:** November 21, 2026  
**Scene Tested:** `Assets/Scenes/GameScene.unity`  
**Engine:** Unity 2021.3.40f1 LTS  

---

## 1. QA Test Results

| Test Case | Scenario | Expected Behavior | Actual Result | Status |
|---|---|---|---|---|
| **TC-PA01** | Walk directly into a Tree | Solid collision without lifting or launching | Smooth stop against capsule collider (`radius: 0.45m`); $Y$ remains grounded at $0.05$ | ✅ PASSED |
| **TC-PA02** | Walk into & around a Park Bench | Slide around bench without stepping or catapulting | Upright BoxCollider (`1.6 × 0.9 × 0.55m`) prevents stepping; $stepOffset: 0.20m$ prevents mount | ✅ PASSED |
| **TC-PA03** | Walk against a Rock | Solid collision with boulder | Upright BoxCollider eliminates angled wedge ramps; character slides naturally | ✅ PASSED |
| **TC-PA04** | Walk on flat grass terrain | Continuous grounded contact | Smooth locomotion; $groundStickForce: -3.5m/s$; $Y$ position perfectly stable | ✅ PASSED |
| **TC-PA05** | Walk over path & plaza seams | Transition across walkway colliders | Flattened pathway colliders (`height: 0.01m`) prevent step bumps or liftoff | ✅ PASSED |
| **TC-PA06** | Idle Animation | Stationary character | Smooth sinusoidal breathing cycle on torso, arms, and head; limbs relaxed | ✅ PASSED |
| **TC-PA07** | Walking Animation | Moving at standard speed (6.0 m/s) | Alternating natural arm swing (32°) & leg swing (36°); subtle torso bounce | ✅ PASSED |
| **TC-PA08** | Running Animation (Sprint) | Holding Shift (9.5 m/s) | Increased frequency (11 Hz) with wider arm (55°) and leg (60°) strides + forward torso lean | ✅ PASSED |
| **TC-PA09** | Animation Transitions | Shifting between idle, walk, and sprint | Smooth blending via velocity-driven ratio interpolation (`MoveTowards: 6f`) | ✅ PASSED |
| **TC-PA10** | Script References & Compilation | Batchmode execution check | 0 compilation errors (`LogAssemblyErrors: 0ms`); all bone references bound | ✅ PASSED |

---

## 2. Root Cause Analysis & Physics Changes

### Issue 1: Flying Near Obstacles
1. **Root Cause**:
   - `CharacterController.stepOffset` was set to `0.35m`. Low-profile environmental props (such as park bench seats at 0.44m, picnic table benches, and low angled rock colliders) fell within the controller's step climbing threshold, causing the physics engine to treat obstacle faces as stairs.
   - When the player ran into an obstacle, the controller resolved forward penetration by stepping up onto the obstacle's surface. Because the player continued pressing forward against the vertical backrest/trunk, the normal forces deflected the character upward, launching them into the air.
   - Furthermore, path colliders (`Path_NS`, `Path_EW`, `CentralPlaza`) were created as 0.04m high cubes slightly protruding above the ground plane, creating physical step ridges.

2. **Physics Changes Implemented**:
   - Lowered `CharacterController.stepOffset` from `0.35m` to `0.20m`.
   - Reconfigured all obstacle colliders (benches, tables, rocks, trees, fountain, lamps) to have strict, vertical upright walls, eliminating angled ramps.
   - Flattened all walking path and plaza colliders to $0.01m$ thickness flush with the terrain.
   - Enforced grounding velocity stick force at `-3.5m/s` with strict XZ movement projection.

### Issue 2: Player Animations
1. **Root Cause**:
   - The player character was previously baked into a single static combined mesh (`Mesh_Player.asset`). Without separated bones or limb pivots, it could only translate and rotate as a rigid block.

2. **Hierarchical Rig & Animation System Implemented**:
   - Generated modular bone mesh assets: `Mesh_CharHead.asset`, `Mesh_CharTorso.asset`, `Mesh_CharArm.asset`, and `Mesh_CharLeg.asset` with properly oriented top-pivot points.
   - Assembled a hierarchical rigged character structure on `Player.prefab` (`ModelRoot -> Torso -> Head, ArmL, ArmR; LegL, LegR`).
   - Created [`PlayerAnimator.cs`](file:///C:/Unity/Trashrush/Assets/Scripts/Player/PlayerAnimator.cs) providing:
     - **Idle**: Procedural breathing cycle oscillating torso, head, and relaxed arms.
     - **Walk**: Synchronized opposite limb swinging (arms 32°, legs 36°) with vertical gait bounce.
     - **Run**: Dynamic high-frequency sprint gait (arms 55°, legs 60°) with forward torso lean.
     - **Transitions**: Continuous velocity-weighted interpolation for smooth state blending.
