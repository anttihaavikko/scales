using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill pool", menuName = "Skill pool", order = 0)]
public class SkillPool : ScriptableObject
{
    [SerializeField] private List<SkillDefinition> definitions;

    public IEnumerable<SkillDefinition> All => definitions;

    public Skill Get(IEnumerable<Skill> existing)
    {
        return Get(1, existing).First();
    }

    public IEnumerable<Skill> Get(int amount, IEnumerable<Skill> existing)
    {
        return definitions.Where(s => s.CanGet(existing)).RandomOrder().Take(amount).Select(d => d.Spawn());
    }
}