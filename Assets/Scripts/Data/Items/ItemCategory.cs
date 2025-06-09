public enum ItemCategory
{
    RawResource,     // e.g., Iron Ore, Copper Ore, Coal
    Intermediate,    // e.g., Iron Plate, Copper Wire
    Component,       // e.g., Gear, Circuit
    Product,         // e.g., Conveyor Belt, Inserter
    Fuel,            // e.g., Coal, Solid Fuel
    Fluid,           // e.g., Water, Oil (future-proofing)
    Tool,            // e.g., Wrench, Scanner (if tools ever exist)
    Special,         // e.g., Research Pack, Key Item
    Other            // Fallback catch-all
}
