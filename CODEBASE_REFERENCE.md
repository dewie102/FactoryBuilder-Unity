# FactoryBuilder — Codebase Reference

> Organized by **capability**, not by file. Before implementing anything new, scan here first.
> Update this file at the end of every session where new methods or classes are added.

---

## Open TODOs

| Location | Note |
|---|---|
| `EntitySystem/Production/MachineEntity.cs` | Add output directions once machine output is implemented |
| `Core/WorldManager.cs` | `ItemTransferred` event is defined but never fired — decide if it's needed for item movement animations or remove it |

---

## Spatial / Grid

| Method | Signature | Notes |
|---|---|---|
| `WorldManager.GetEntityAt` | `(Vector3Int) → Entity` | Primary way to read world state. Returns null if empty. |
| `WorldManager.HasEntityAt` | `(Vector3Int) → bool` | Quick existence check. |
| `WorldManager.GetAllEntities` | `() → IEnumerable<KVP<Vector3Int, Entity>>` | Full snapshot. Used by ChainDetector on rebuild. |
| `WorldManager.GetNeighborPositionInDirection` | `(Vector3Int, Direction) → Vector3Int` | Position one step in a direction. Use when you need the position itself (e.g. chain tracing). |
| `WorldManager.GetNeighborEntityInDirection` | `(Vector3Int, Direction) → Entity` | Entity one step in a direction. Use when you only need the entity (most cases). |
| `WorldManager.GetNeighborEntities` | `(Vector3Int) → Dictionary<Direction, Entity>` | All 4 cardinal neighbors as Direction → Entity map. Used by Rotate and OnPlaced. |
| `WorldManager.GetNeighbors` | `(Vector3Int) → List<Vector3Int>` | All 4 cardinal neighbor positions. Kept for cases where only positions are needed. |
| `WorldManager.WorldToCell` | `(Vector3) → Vector3Int` | World position → grid cell. |
| `WorldManager.CellToWorld` | `(Vector3Int) → Vector3` | Grid cell → centered world position (adds half cellSize). |

---

## Entity System

### Hierarchy
```
Entity                          (base — has EntityData, OnTick, Rotate, OnPlaced)
└── ItemHolderEntity            (abstract — holds one item, fires events)
    ├── ConveyorEntity          (IItemConsumer, IItemProducer, IChainableEntity)
    ├── ResourceNodeEntity      (IItemProducer)
    └── MachineEntity           (IItemConsumer)
```

### Key methods

| Method | Location | Notes |
|---|---|---|
| `Entity.OnTick` | `EntitySystem/Entity.cs` | Virtual. Called every simulation tick for all entities. |
| `Entity.Rotate` | `EntitySystem/Entity.cs` | Virtual. Called by WorldManager.RotateEntity with neighbor map. |
| `Entity.OnPlaced` | `EntitySystem/Entity.cs` | Virtual. Called by WorldManager.PlaceEntity when an outputDirection is provided. Receives neighbor map. Override to set initial orientation on placement. |
| `ItemHolderEntity.TryConsumeItem` | `EntitySystem/ItemHolderEntity.cs` | The correct way to push an item into an entity. Calls CanConsumeItem internally. |
| `ItemHolderEntity.PeekItem` | `EntitySystem/ItemHolderEntity.cs` | Returns current item without removing it. |
| `ItemHolderEntity.HasItem` | `EntitySystem/ItemHolderEntity.cs` | Bool property. |
| `ConveyorEntity.SetOrientation` | `EntitySystem/Logistics/ConveyorEntity.cs` | Sets input + output direction. Fires DirectionsChanged. |
| `ConveyorEntity.Rotate` | `EntitySystem/Logistics/ConveyorEntity.cs` | Rotates output clockwise, smart-selects input by scanning neighbors for producer pointing at new input side. |
| `ResourceNodeEntity.SetOrientation` | `EntitySystem/Resources/ResourceNodeEntity.cs` | Sets output direction only. |
| `EntityFactory.CreateEntity` | `EntitySystem/EntityFactory.cs` | Switch on EntityType → returns correct concrete entity. **Add new entity types here.** |

### Interfaces

| Interface | Implementors | Key members |
|---|---|---|
| `IItemProducer` | ResourceNodeEntity, ConveyorEntity | `OutputDirections`, `HasItem`, `PeekItem()`, `RemoveItem()` |
| `IItemConsumer` | MachineEntity, ConveyorEntity | `InputDirections`, `TryConsumeItem()`, `CanConsumeItem()` |
| `IChainableEntity` | ConveyorEntity only | Marker interface — empty. Used to distinguish conveyors from producers/consumers during chain detection. |

---

## Conveyor Chains

### How it works
- On entity placed, removed, or rotated → `ChainDetector` scans all non-chainable `IItemProducer`s, traces output through connected conveyors until hitting a non-chainable consumer, builds a `ConveyorChain` per sequence.
- Each tick → `HandleChainConnections()` delivers from chain output to downstream consumer, then `ProcessAllChains()` advances items through each chain back-to-front.

### Key classes and methods

| Method / Property | Location | Notes |
|---|---|---|
| `ConveyorChain.Positions` | `Core/ConveyorChains/ConveyorChain.cs` | `List<Vector3Int>`, insertion-ordered. `[0]` = entry, `[^1]` = exit. |
| `ConveyorChain.InputPosition` | computed | `Positions[0]` |
| `ConveyorChain.OutputPosition` | computed | `Positions[^1]` |
| `ConveyorChain.AdvanceItems` | `Core/ConveyorChains/ConveyorChain.cs` | Walks back-to-front, moves items forward if next slot empty. Pulls from upstream producer into `[0]` if empty. |
| `ChainDetector.DetectChains` | `Core/ConveyorChains/ChainDetector.cs` | Returns `List<ConveyorChain>`. Algorithm: find non-chainable producers → trace output through IChainableEntity → build chain. |
| `ConveyorChainManager.DetectChains` | `Core/ConveyorChains/ConveyorChainManager.cs` | Calls detector, stores result, fires `ChainsDetected` event. |
| `ConveyorChainManager.ProcessAllChains` | `Core/ConveyorChains/ConveyorChainManager.cs` | Calls `AdvanceItems()` on every chain. |
| `ConveyorChainManager.HandleChainConnections` | `Core/ConveyorChains/ConveyorChainManager.cs` | Delivers from chain output into downstream consumer (machine, etc). Runs before `ProcessAllChains`. |
| `ChainsDetected` | `Core/ConveyorChains/ConveyorChainManager.cs` | `static event Action<List<ConveyorChain>>` — fires on topology change. Subscribe here for visual updates. |

---

## Simulation Flow

```
SimulationManager.Update()          every tickInterval seconds (default 2s)
└── WorldManager.TickWorld()
    └── ProcessEntities()
        ├── ConveyorChainManager.HandleChainConnections()   deliver chain output → consumer
        ├── ConveyorChainManager.ProcessAllChains()         advance items through chains
        └── entity.OnTick()                                  for every entity
```

### Placement / removal / rotation flow
```
PlaceEntity(position, entityData, outputDirection?)
├── entity.OnPlaced(outputDirection, neighbors)   if outputDirection provided
├── EntityPlaced event
└── ConveyorChainManager.DetectChains()           rebuild chains

RemoveEntity() or RotateEntity()
└── ConveyorChainManager.DetectChains()           rebuild chains
    └── ChainsDetected event                      notify visual layer
```

---

## Items

| Class / Type | Location | Notes |
|---|---|---|
| `Item` | `Data/Items/Item.cs` | Runtime instance. Equality by `ID`. Created from `ItemData`. |
| `ItemData` | `Data/Items/ItemData.cs` | ScriptableObject. Set `id`, `displayName`, `sprite`, `maxStackSize` in Inspector. |
| `ItemTransfer` | `Data/Structs/ItemTransfer.cs` | Struct: item, from/to position, from/to entity. Used by `ItemTransferred` event. Old transfer queue system removed — event-only now. |

---

## Data / Config

| Type | Location | Notes |
|---|---|---|
| `EntityData` | `Data/Entities/EntityData.cs` | ScriptableObject. Key fields: `id`, `type` (EntityType), `itemToProduce` (ResourceNode), `prefab`. |
| `EntityType` | `Data/Entities/EntityType.cs` | `Conveyor \| Machine \| ResourceNode` — used by EntityFactory switch. |
| `EntityCategory` | `Data/Entities/EntityCategory.cs` | Optional grouping for UI / build menu. |
| `Direction` | `Data/Enums/Direction.cs` | `LEFT \| UP \| RIGHT \| DOWN` — used everywhere for orientation and traversal. |
| `ItemCategory` | `Data/Items/ItemCategory.cs` | Optional grouping for items. |

---

## Utilities

All in `Utilities/DirectionUtils.cs` — all static.

| Method | Notes |
|---|---|
| `DirectionUtils.ToVector3Int(Direction)` | UP→(0,1,0), DOWN→(0,-1,0), LEFT→(-1,0,0), RIGHT→(1,0,0) |
| `DirectionUtils.Reverse(Direction)` | UP↔DOWN, LEFT↔RIGHT. Used to check if a neighbor's output points at you. |
| `DirectionUtils.GetRotatedDirection(Direction)` | Clockwise: RIGHT→UP→LEFT→DOWN→RIGHT |
| `DirectionUtils.TryGetDirection(Vector3Int, out Direction)` | Vector back to Direction. Returns false if not cardinal. |

---

## Events Reference

Subscribe to these for visual layer updates — don't poll.

| Event | Source | Payload | When it fires |
|---|---|---|---|
| `WorldManager.EntityPlaced` | `Core/WorldManager.cs` | `Vector3Int, Entity` | After entity added to world |
| `WorldManager.EntityRemoved` | `Core/WorldManager.cs` | `Vector3Int` | After entity removed |
| `WorldManager.EntityRotated` | `Core/WorldManager.cs` | `Vector3Int, Entity` | After rotation completes |
| `WorldManager.ItemTransferred` | `Core/WorldManager.cs` | `ItemTransfer` | **Never fired currently** — placeholder for item movement animations |
| `ConveyorChainManager.ChainsDetected` | `Core/ConveyorChains/ConveyorChainManager.cs` | `List<ConveyorChain>` | After any chain rebuild (place, remove, or rotate) |
| `ItemHolderEntity.ItemAdded` | per-entity | `Item` | When entity receives an item |
| `ItemHolderEntity.ItemRemoved` | per-entity | _(none)_ | When item cleared from entity |
| `ItemHolderEntity.DirectionsChanged` | per-entity | _(none)_ | After SetOrientation / rotation |

---

## What doesn't exist yet (next big areas)

- **Recipes** — MachineEntity currently just deletes items. No recipe data, no input matching, no multi-item consumption.
- **Tick system refinement** — SimulationManager tick is uniform for all entities. No per-entity tick rates.
- **Mergers / Splitters** — Chain boundary entities. Would be chain endpoints that connect multiple chains.
- **Visual item movement** — `ItemTransferred` event exists but nothing animates items moving across conveyors.
- **Build UI** — Entity selection currently via number keys (1–9 mapped to EntityLibrary). No toolbar panel yet.

---

*Last updated: session June 2026*
