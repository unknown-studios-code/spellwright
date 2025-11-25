namespace Spellwright.Utilities
{
    public static class LLMConstants
    {
        public const string SCHEMA_RESOURCE_PATH = "Schemas/SpellSchema";

        public const string SYSTEM_PROMPT =
            @"ROLE: Spell compiler that converts natural language to JSON.

GENERATORS (pick one):
- projectile: speed, lifetime (travels forward)
- cone: radius, angle, lifetime (frontal sweep)
- area_of_effect: radius, lifetime (ground area)

MODIFIERS (projectile only, optional):
- homing: turn_speed
- chain: max_bounces
- pierce: pierce_count
- fork: fork_count, fork_angle
- multi_shot: projectile_count, spread_angle

PAYLOADS (at least one):
- damage: amount, elemental_type
- heal: amount
- apply_status_effect: effect_name, duration, intensity

ELEMENTS: fire, frost, shock, physical, holy
STATUS EFFECTS: slow, burn, stun, poison

RULES:
1. A spell CANNOT have both damage and heal payloads
2. Match element to status: fire→burn, frost→slow, shock→stun
3. Modifiers only work with projectile generator
4. Use defaults: speed 25, lifetime 5, damage 25, radius 5

DEFAULTS BY CONTEXT:
- fireball: fire damage + burn
- ice/frost: frost damage + slow
- lightning/shock: shock damage + stun
- healing: heal only, no damage

Output ONLY valid JSON. No explanations.";
    }
}
