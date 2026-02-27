using UnityEngine;

// public class CharacterPositionRule : IValidationRule<CharacterData>
// {
//     public bool Validate(CharacterData character, out string error)
//     {
//         if (float.IsInfinity(character.Pos.Value.x) ||
//             float.IsInfinity(character.Pos.Value.y) ||
//             float.IsInfinity(character.Pos.Value.z)
//         )
//         {
//             error = "Character position contains invalid values.";
//             return false;
//         }

//         error = null;
//         return true;
//     }
// }

// public class CharacterDirectionRule : IValidationRule<CharacterData>
// {
//     public bool Validate(CharacterData character, out string error)
//     {
//         if (float.IsInfinity(character.Direction.x) ||
//             float.IsInfinity(character.Direction.y) ||
//             float.IsInfinity(character.Direction.z)
//         )
//         {
//             error = "Character direction is invalid.";
//             return false;
//         }

//         error = null;
//         return true;
//     }
// }

// public class CharacterLootRule : IValidationRule<CharacterData>
// {
//     public bool Validate(CharacterData character, out string error)
//     {
//         if (character.Loot == null)
//         {
//             error = "Character loot data is missing.";
//             return false;
//         }

//         error = null;
//         return true;
//     }
// }