# Unity Grid & Pathfinding System

This project is a high-performance, modular **Grid Construction, Level Physics Detection, and Asynchronous A* Pathfinding System** built on the **Unity 6 (6000.3.8f1)** engine. The project showcases dynamic multi-layer object construction, separable box blur map weight smoothing, heap-optimized A* pathfinding, and analytical geometry boundary checks for NPC movement control.

---

## 🎮 How to Play / Controls

Once the game starts, you can control the camera, building modes, and NPC pathfinding using the keyboard and mouse:

### 1. Camera Controls
*   **Camera Movement (Pan)**: Use the `W`, `A`, `S`, `D` keys, or press and hold the **Middle Mouse Button (Scroll Click)** and drag the mouse.
*   **Camera Rotation**: Use the `Q`, `E` keys, or press and hold the **Right Mouse Button** and drag the mouse (the camera rotates around its ground projection focal point).
*   **Camera Zoom**: Scroll the **Mouse Wheel** to zoom in or out (smoothly interpolated via FOV).

### 2. Build & Remove Mode
*   **Select Buildings**: Click any building button on the UI to enter **Placing Mode**. A semi-transparent building preview will follow your cursor.
*   **Rotate Objects**: In Placing Mode, press the `R` key or `Tab` key to rotate the object clockwise in 90-degree increments.
*   **Place Objects**: **Left Click** to place the object onto the targeted grid cell.
*   **Remove Objects**: Click the **Remove Mode Button** on the UI, hover over the grid containing the object you want to remove, and **Left Click** to delete the topmost object of that grid cell.
*   **Switch/Cancel Current Mode**: Click other tool/mode buttons on the UI to switch.

### 3. NPC Pathfinding Mode
*   Click the **Placing NPC Button** on the UI.
*   **Step 1 (Spawn NPC)**: Click any walkable grid area to spawn or relocate the NPC character.
*   **Step 2 (Assign Destination)**: Click another walkable grid cell. The system will automatically compute the optimal path, and the NPC will move along the path (incorporating smooth corner turning and deceleration) to the destination.
*   **Repeat**: Click again to reset the NPC's spawn point and destination.

---

## 🛠️ Core Systems & Technical Highlights

```
┌─────────────────────────────────────────────────────────────┐
│                       TestPlayer                            │
└──────────────┬──────────────────────────────┬───────────────┘
               │ (Placing / Removing)         │ (Set Start / End)
               ▼                              ▼
┌──────────────────────────────┐        ┌─────────────────────┐
│       GridBuildManager       │        │  NpcPathFindSpawner │
└──────────────┬───────────────┘        └─────────────┬───────┘
               │                                      │ (Spawn / Target)
               ▼                                      ▼
┌──────────────────────────────┐        ┌─────────────────────┐
│          Grid System         │        │       TestNPC       │
└──────────────┬───────────────┘        └─────────────┬───────┘
               │                                      │ (RequestPath)
               │ (Event Broadcast)                    ▼
               │                        ┌─────────────────────┐
               └───────────────────────>│  PathRequestManager │
                                        └─────────────┬───────┘
                                                      │ (ThreadPool Calc)
                                                      ▼
                                        ┌─────────────────────┐
                                        │    PathFinding      │
                                        │ (Heap-Optimized A*) │
                                        └─────────────────────┘
```

1.  **Dynamic Multi-Layer Construction System**
    *   Supports object placement in four directions with dynamic rotation, automatically correcting grid footprint bounds and world anchor offsets.
    *   Features a multi-layer grid stacking mechanism (e.g., floors, walls, furniture), seamlessly restoring underlying grid node properties when the topmost item is removed.
2.  **Separable Box Blur Path Penalty Smoothing**
    *   Applies a separable image box blur filter at game initialization to smooth grid penalties around obstacles. Achieves a time complexity of $O(N)$ (linear relative to grid size), allowing NPC navigation paths to naturally steer away from sharp wall corners.
3.  **Static Level Physics Detection (Raycast Mesh Analyzer)**
    *   Scans and detects objects marked with `TileCategoryMarker` at startup, automatically generating walkability properties and penalty costs for grid cells to save level design overhead.
4.  **Heap-Optimized A\* Pathfinding & Multi-Threaded Queue**
    *   Implements a custom generic binary min-heap `Heap<T>` using an array backbone to reduce Open Set insertion, update, and extraction complexity to $O(\log N)$.
    *   Offloads pathfinding operations asynchronously to background worker threads, preventing frame rate stutters, and simplifies paths by removing collinear redundant waypoints.
5.  **Analytical Geometry NPC Path-Following Controller**
    *   Uses 2D analytical geometry and half-space line boundary checks (relative side tests) rather than simple distance thresholds to trigger waypoint transitions, preventing NPCs from missing waypoints due to framerate drops.
    *   Integrates the **UniTask** asynchronous framework to safely cancel tasks in sync with the NPC object's lifecycle.

---

## 📂 Folder Directory

```
Assets/Grid Game/
├── Art/                     # 3D assets, materials, and post-processing Volume profiles
├── Game Settings/           # Level configuration and tile penalty settings ScriptableObjects
├── Input System/            # Unity Input System settings (User Control & Bindings)
├── Prefab/                  # GameManager, UI, NPC, and building prefabs
├── Scenes/                  # Main demonstration scene (SampleScene)
├── Scriptable Objects/      # Building metadata listings (PlaceableData, TiledItemList)
└── Scripts/                 # Core C# source code
    ├── Build System/        # Building construction, multi-layer snapping, and placement logic
    ├── Event System/        # Struct-based decoupled typed event bus
    ├── Game/                # GameManager, camera controls, player input state machine
    ├── Grid System/         # 2D Grid data structures and A* heap-optimized pathfinding core
    ├── NPC/                 # NPC path-following movement states and UniTask movement sequences
    ├── SonaruUtilities/     # Custom utilities (binary heap, object pooling adaptors)
    └── UI/                  # UI button and screen binding registration logic
```

---

## ⚙️ Requirements & Dependencies

*   **Unity Editor Version**: `Unity 6 (6000.3.8f1)` or newer
*   **Render Pipeline**: Universal Render Pipeline (**URP** 17.3.0)
*   **Input System**: New Unity **Input System** (1.18.0)
*   **Package Dependencies**:
    *   [UniTask](https://github.com/Cysharp/UniTask) (Asynchronous multi-threaded coroutine wrapper)
    *   [UtilSNR](https://github.com/SonaruIsuge/UtilSNR.git) (Contains object pooling `PoolManager` and singleton base `TSingletonBehaviour`)
    *   NaughtyAttributes (Inspector workflow enhancement utility)
