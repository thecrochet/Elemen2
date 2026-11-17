// The elements of the game. Used by the ball, the gates, the particles and the materials.
//
// IMPORTANT: Unity saves enums as NUMBERS. Never reorder this list and never insert in the middle:
// only ADD new elements at the END, or every gate you already placed changes element.
public enum PowerUpType
{
    Fire,     // dash (breaks blocks)
    Air,      // double jump + glide
    Earth,    // ground pound (+ super jump from a bounce pad)
    Water,    // wall jump + vapor (gravity flip)
    None,     // the base ball: no power (its look is the electric spark)
    Aether    // the dark power of the final levels (no ability yet: the look is ready)
}
