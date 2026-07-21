# Unity Project Architecture Analysis - Worlds Hardest Maker

## Executive Summary
This is a Unity level editor project with a **saturated Manager-based architecture**. The codebase suffers from excessive coupling, poor separation of concerns, and a "god object" anti-pattern proliferation. Below are concrete problems and actionable solutions.

---

## Critical Architectural Problems

### 1. **Manager Explosion (20+ Singleton Managers)**

**Problem:** The project has 20+ global singletons, all with `public static Instance { get; private set; }`:
- GameManager, ReferenceManager, LayerManager, MaterialManager, MouseManager, PanelManager, PickManager, PlaceManager, PlayManager, PlayerRecordingManager, PrefabManager, TextManager, TransitionManager, UndoManager
- LevelSessionManager, LevelSessionEditManager, LevelCompleteManager
- DiscordManager, KonamiManager
- PickManager, SelectionManager (partial: 5 files), AnchorManager (partial: 3 files), AnchorAttachManager, FieldManager (partial: 2 files), etc.

**Impact:**
- Hard to test (all singletons must be initialized)
- Hard to reason about dependencies (any manager can call any other)
- Global state makes debugging difficult
- Code reuse impossible (managers are tightly coupled to specific scenes/contexts)

**Solution - Event/Service Bus Architecture:**
```csharp
// Instead of: GameManager.Instance.LoadLevel()
// Use: EventBus.Publish(new LevelLoadRequest(path));

// Or dependency injection:
public class LevelLoader {
    private readonly ISaveService _saveService;
    private readonly ILevelRenderer _renderer;
    
    public LevelLoader(ISaveService save, ILevelRenderer render) {
        _saveService = save;
        _renderer = render;
    }
}
```

**Immediate Action:** Introduce a **SceneContext** pattern or **Zenject** (dependency injection framework) to replace global singletons gradually.

---

### 2. **Partial Classes Overuse (God Object Anti-pattern)**

**Problem:** Classes split into 2-5 partial files with unclear responsibility:
- `SelectionManager` (5 files: base + Preview, Outline, Fill, Actions)
- `AnchorManager` (3 files: base + Select, SetGet, Warnings)
- `FieldManager` (2 files: base + Intersection, Neighbor)
- `LevelCompleteManager` (2 files: base + Animation)
- `PlayerController` (4 files: base + Death, FieldDetection, Movement, GameState, VoidDetection)

**Why it's bad:**
- Suggests single class is too large (SRP violation)
- Code navigation difficult (ctrl+click doesn't follow logic well)
- Obscures tight coupling between responsibilities
- Partial classes used as "workaround" instead of fixing design

**Correct Solution - Composition Over Inheritance:**
```csharp
// Instead of: SelectionManager (partial x5) with 2000+ lines
public class SelectionManager {
    private SelectionOutlineRenderer _outlineRenderer;
    private SelectionPreviewRenderer _previewRenderer;
    private SelectionFillCalculator _fillCalculator;
    private SelectionActionHandler _actionHandler;
    
    // Clean, single responsibility
}

// Split into focused classes:
public class SelectionOutlineRenderer { /* rendering logic */ }
public class SelectionPreviewRenderer { /* preview logic */ }
public class SelectionFillCalculator { /* fill calculation */ }
public class SelectionActionHandler { /* user actions */ }
```

**Immediate Action:** Refactor each 3+ partial-file class into a Composite/Manager that delegates to helper classes.

---

### 3. **Circular Dependencies Between Managers**

**Problem:** Managers reference each other without clear dependency direction:
- `PlaceManager.cs` line 37: `FieldManager.Instance.Place()`
- `PlaceManager.cs` line 48: `PlayerManager.Instance.RemoveAtPosIntersectInSheet()`
- `PlaceManager.cs` line 58-71: Loop through all managers (PlayerManager, BallManager, CoinManager, AnchorManager, KeyManager)
- `GameManager.cs`: References ReferenceManager, LevelSessionManager, TransitionManager, AnchorManager, PlayerManager
- `SaveSystem.cs`: Calls AnchorManager, PlayerManager, BallManager, CoinManager, KeyManager directly

This creates a **spaghetti dependency graph**.

**Solution - Dependency Graph Clarity:**
```csharp
// Define clear input → processing → output flow:
// UserInput → InputManager
// InputManager → CommandProcessor
// CommandProcessor → appropriate Handler (PlacementHandler, SelectionHandler, etc)
// Handlers → Data Services (SaveSystem, FieldManager, etc)

// Use command pattern:
public interface ICommand {
    void Execute();
    void Undo();
}

public class PlaceEntityCommand : ICommand {
    // Takes only what it needs, doesn't access global managers
    public PlaceEntityCommand(Vector2 pos, EditMode mode, IFieldService field, IEntityService entity) { }
}
```

**Immediate Action:** Map the dependency graph. Identify "hub" managers that coordinate too many things and split them.

---

### 4. **No Clear Layer Separation**

**Problem:** Code doesn't distinguish between:
- **UI Layer** - Panels, buttons, overlays
- **Game Logic Layer** - Game state, rules, placement
- **Data Layer** - Serialization, persistence, models
- **Presentation Layer** - Rendering, visual feedback

Example mixed responsibilities:
- `TextManager.cs`: Updating UI text directly, accessing PlayerManager, CoinManager for data
- `PlaceManager.cs`: Takes EditMode, plays sounds, updates multiple entity managers
- `PlayerRecordingManager.cs`: Records positions, renders paths, renders sprites

**Solution - Layered Architecture:**
```
Presentation Layer (UI Controllers, Animators)
     ↓ (updates only)
Business Logic Layer (Services, Commands, State)
     ↓ (dependency on)
Data Layer (Models, Repositories, Serialization)

Rule: Never go upward in this hierarchy
```

**Immediate Action:** Create folders:
- `Scripts/UI/` - Only UI controls, no game logic
- `Scripts/Services/` - Stateless business logic
- `Scripts/Data/` - Models and persistence
- `Scripts/GameLogic/` - Game rules and state

---

### 5. **SaveSystem is a Static Utility with Hidden Dependencies**

**Problem:** `SaveSystem.cs` is a `public static class` that:
- Uses reflection: `MethodInfo.Invoke()` on managers (lines 78)
- Directly accesses singleton instances (AnchorManager, PlayerManager, etc)
- Couples serialization tightly to specific manager implementations
- No abstraction for what should be serializable

```csharp
// Current: reflection-based hacks
List<IManager> managers = new() { PlayerManager.Instance, BallManager.Instance, ... };
foreach (IManager manager in managers) {
    Type type = manager.GetType();
    MethodInfo methodInfo = type.GetMethod(nameof(IManager<LevelObjectController>.Serialize));
    levelData = (List<Data>)methodInfo!.Invoke(manager, new object[] { levelData, });
}
```

**Solution - Explicit Serializer Pattern:**
```csharp
public interface ILevelSerializer {
    LevelData Serialize(Level level);
    Level Deserialize(LevelData data);
}

public class LevelSerializer : ILevelSerializer {
    private readonly IPlayerSerializer _playerSerializer;
    private readonly IBallSerializer _ballSerializer;
    // Inject actual serializers
    
    public LevelData Serialize(Level level) {
        return new LevelData {
            Objects = new List<Data> {
                _playerSerializer.Serialize(level.Player),
                _ballSerializer.Serialize(level.Balls),
                // ... etc
            }
        };
    }
}

// In managers:
public interface ISerializable {
    Data SerializeToData();
    void DeserializeFromData(Data data);
}
```

**Immediate Action:** Replace reflection-based serialization with explicit type-safe serializers.

---

### 6. **IManager Interface is Underutilized & Generic Confusion**

**Problem:**
- `IManager` interface is broad and non-specific (mixes concerns)
- Generic `IManager<T>` requires reflection workarounds
- Multiple implementations of same interface scattered across folders
- No clear contract about what each manager type does

```csharp
// Current confused design:
public interface IManager {
    public bool CorrespondsToEditMode(EditMode compare); // UI concern?
}

public interface IManager<out T> : IManager where T : LevelObjectController {
    public T Set(ManagerParameters args);
    public T SetInSheet(ManagerParameters args);
    // ... 7 methods total
}

// Then reflection hacks to invoke these
```

**Solution - Role-based Interfaces:**
```csharp
// Clear, specific responsibilities:
public interface IPlaceable {
    void PlaceAt(Vector2 position);
}

public interface ISelectable {
    void Select(Vector2 position);
}

public interface ISerializable {
    Data Serialize();
}

public interface IEditable {
    EditMode GetEditMode();
}

// Managers implement what they actually do
public class PlayerManager : IPlaceable, ISelectable, ISerializable { }
public class FieldManager : IPlaceable, IEditable { }
```

**Immediate Action:** Replace generic `IManager<T>` with role-based interfaces.

---

### 7. **ReferenceManager is a Massive Locator Anti-pattern**

**Problem:** ReferenceManager has 50+ public fields storing UI elements, containers, prefabs:
```csharp
public Canvas Canvas;
public GameObject TooltipCanvas;
public GameObject Menu;
public AlphaTween MenuTween;
public PlacementPreviewController PlacementPreview;
// ... and 40+ more fields
```

This is a **Service Locator anti-pattern** that hides dependencies.

**Solution - Constructor Injection:**
```csharp
// Bad (hidden dependency via locator):
public class UIPanel : MonoBehaviour {
    void Start() {
        canvas = ReferenceManager.Instance.Canvas;
        tooltip = ReferenceManager.Instance.TooltipCanvas;
    }
}

// Good (explicit dependency):
public class UIPanel : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject tooltipCanvas;
    // Dependencies are obvious in inspector
}

// For programmatic scenarios, use proper DI container (Zenject)
```

**Immediate Action:** Gradually move ReferenceManager fields to individual scripts via SerializeFields.

---

### 8. **No Clear Error Handling or State Validation**

**Problem:**
- `SaveSystem.cs` checks `if (path.Equals(""))` with warning but doesn't prevent action
- `UndoManager.cs` catches generic Exception and silently continues
- No validation of manager state before operations
- `TextManager.cs` has try-catch that just assigns "-" on error

```csharp
// Example: Silent failure
try {
    PlayerController currentPlayer = PlayerManager.Instance.Player;
    playerDeaths = currentPlayer.Deaths;
} catch (Exception) {
    // no player placed - just show "-"
    playerDeaths = "-";
}
```

**Solution - Explicit State and Validation:**
```csharp
public class GameSession {
    private PlayerController _player;
    
    public bool TryGetPlayer(out PlayerController player) {
        player = _player;
        return _player != null;
    }
    
    public void SetPlayer(PlayerController player) {
        if (player == null) throw new ArgumentNullException(nameof(player));
        _player = player;
    }
}

// Usage:
if (_session.TryGetPlayer(out var player)) {
    deaths = player.Deaths;
} else {
    deaths = 0; // explicit handling
}
```

**Immediate Action:** Add validation to critical operations, throw meaningful exceptions.

---

### 9. **No Clear Event Flow or Message Bus**

**Problem:** 
- Managers subscribe to events but the event sources are scattered
- No centralized event definitions
- Hard to track data flow (e.g., what happens when level loads?)

Current pattern: `PlayManager.Instance.OnSwitchToPlay += SwitchToPlay;`

**Solution - Centralized Event Bus:**
```csharp
public class GameEventBus {
    public event Action<LevelData> OnLevelLoaded;
    public event Action OnPlayModeEntered;
    public event Action OnEditModeEntered;
    
    public void PublishLevelLoaded(LevelData data) => OnLevelLoaded?.Invoke(data);
    public void PublishPlayModeEntered() => OnPlayModeEntered?.Invoke();
}

// Usage:
public class PlayerRecordingManager {
    public PlayerRecordingManager(GameEventBus eventBus) {
        eventBus.OnPlayModeEntered += StartRecording;
    }
}
```

**Immediate Action:** Create a `GameEventBus` for all major events (mode switches, level load, etc.).

---

### 10. **Editor Code Mixed with Runtime Code**

**Problem:** `LayerManager.cs` contains `#if UNITY_EDITOR` directives mixing concerns:
```csharp
#if UNITY_EDITOR
[ButtonMethod]
public void UpdateSortingLayerLists() { ... }
#endif
```

**Solution - Separate Files:**
```
LayerManager.cs (runtime)
LayerManagerEditor.cs (editor, inherits from LayerManager)
```

**Immediate Action:** Move all editor code to Editor/ folder with `Editor.cs` suffix.

---

## Redundancies & Unconventional Patterns

### 1. **Multiple Singleton Implementations**
Every manager reimplements singleton pattern:
```csharp
public static GameManager Instance { get; private set; }
private void Awake() {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
}
```

**Better:** Use a Singleton base class:
```csharp
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance { get; private set; }
    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}

public class GameManager : SingletonMonoBehaviour<GameManager> { }
```

### 2. **Static Classes for Configuration**
`LayerManager` stores LayerMask and SortingLayer names in public serialized fields. Better to use ScriptableObject:
```csharp
[CreateAssetMenu(menuName = "Config/Layer Config")]
public class LayerConfig : ScriptableObject {
    public LayerMask Entity;
    public LayerMask Player;
    // ... etc
}
```

### 3. **Magic Strings and Keybinds**
Everywhere: `KeyBinds.GetKeyBindDown("Editor_Undo")`

Better: Use enum:
```csharp
public enum KeyAction { EditorUndo, EditorSelect, EditorPlace }
public class KeyBindManager {
    public bool GetKeyBindDown(KeyAction action) { ... }
}
```

### 4. **Conditional Logic Scattered**
`SelectionManager.Update()` has nested conditions checking multiple manager states:
```csharp
if (!LevelSessionManager.Instance.IsEdit
    || AnchorAttachManager.Instance.InAttachMode) return;

if (KeyBinds.GetKeyBind("Editor_Select") && ...)
```

Better: Extract to "CanStartSelection()" method:
```csharp
if (!CanStartSelection()) return;
```

### 5. **Type Casting Without Validation**
`PlaceManager.cs` line 36: `FieldMode mode = (FieldMode)editMode;`

Unsafe cast. Better:
```csharp
if (editMode is FieldMode fieldMode) {
    FieldManager.Instance.Place(fieldMode, ...);
}
```

### 6. **Inconsistent Naming**
- `AnchorManager` vs `BallManager` (what's the pattern?)
- `OnSwitchToPlay` vs `OnEditAction` (different naming convention)
- `PlaceManager.Place()` vs `FieldManager.Remove()` (inconsistent CRUD verb)

Standard: Use `Create`, `Read`, `Update`, `Delete` or `Add`, `Get`, `Update`, `Remove`

---

## Recommended Refactoring Roadmap

### Phase 1: Foundation (1-2 weeks)
1. Introduce `SceneContext` for dependency injection
2. Create `GameEventBus` for central event publishing
3. Create base `SingletonMonoBehaviour<T>` to reduce boilerplate
4. Separate Editor code into Editor/ folder

### Phase 2: Despaghettify Dependencies (2-3 weeks)
1. Map dependency graph with Mermaid/Graphviz
2. Create `CommandPattern` for user actions
3. Replace reflection-based SaveSystem with explicit serializers
4. Introduce `ILevelService` abstraction over SaveSystem

### Phase 3: Decompose God Objects (2-3 weeks)
1. Split SelectionManager into: SelectionState, SelectionRenderer, SelectionController
2. Split AnchorManager into: AnchorState, AnchorRenderer, AnchorController
3. Split PlayerController into: PlayerState, PlayerPhysics, PlayerAnimation, PlayerInput

### Phase 4: Layer Separation (2 weeks)
1. Create dedicated UI/, Services/, Data/ folders
2. Move business logic out of MonoBehaviours
3. Keep MonoBehaviours as thin view-models only

### Phase 5: Testing Infrastructure (1 week)
1. Extract testable services without MonoBehaviour dependency
2. Add unit tests for serialization
3. Add integration tests for command flow

---

## Quick Wins (Can do immediately)

1. **Add Region comments** to organize huge classes by responsibility
2. **Extract private methods** (SelectionManager.Update is 30 lines)
3. **Move constants to top** instead of magic numbers scattered in code
4. **Add XML docs** to public APIs (especially IManager implementations)
5. **Replace magic strings** like "Editor_Undo" with enums

---

## Tools to Consider

- **Zenject** - Dependency injection container for Unity (eliminates singletons)
- **UniRx** - Reactive programming (reactive event bus)
- **MediatR** - Command/query pattern for decoupling
- **NUnit** - Unit testing framework
- **dotTrace** - Find circular dependencies and slow code paths

---

## Summary

The codebase is functional but has **low maintainability** due to:
1. Manager explosion (20+ singletons)
2. Partial class overuse masking design problems
3. Circular dependencies between managers
4. No clear layer separation
5. Service locator anti-patterns (ReferenceManager)

The fix is **composition-based architecture** with clear dependency flow and explicit responsibilities. Start with dependency injection and event bus to break up the circular dependencies.
