#nullable enable
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Types.Entities.Level;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Users;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Types.Categories;

public class NewestLevelsCategory : Category
{
    public override string Name { get; set; } = "Newest Levels";
    public override string Description { get; set; } = "The most recently published content";
    public override string IconHash { get; set; } = "g820623";
    public override string Endpoint { get; set; } = "newest";
    public override Slot? GetPreviewSlot(DatabaseContext database) => database.Slots.Where(s => s.Type == SlotType.User).OrderByDescending(s => s.FirstUploaded).FirstOrDefault();
    public override IEnumerable<Slot> GetSlots
        (DatabaseContext database, int pageStart, int pageSize)
        => database.Slots.ByGameVersion(GameVersion.LittleBigPlanet3, false, true)
            .OrderByDescending(s => s.FirstUploaded)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 20));
    public override int GetTotalSlots(DatabaseContext database) => database.Slots.Count(s => s.Type == SlotType.User);
}