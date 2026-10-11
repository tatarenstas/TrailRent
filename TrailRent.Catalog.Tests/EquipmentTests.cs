using TrailRent.Catalog.Domain;
using Xunit;

namespace TrailRent.Catalog.Tests;

public sealed class EquipmentTests
{
    [Fact]
    public void RemoveFromStock_rejects_more_units_than_available()
    {
        var equipment = new Equipment("TENT-01", "Tent", 300m, 1, 1);
        Assert.Throws<ConflictException>(() => equipment.RemoveFromStock(2));
    }

    [Fact]
    public void ChangeDailyRate_rejects_zero()
    {
        var equipment = new Equipment("TENT-01", "Tent", 300m, 1, 1);
        Assert.Throws<ValidationException>(() => equipment.ChangeDailyRate(0));
    }
}
