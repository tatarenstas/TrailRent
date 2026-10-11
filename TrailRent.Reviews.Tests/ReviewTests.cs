using TrailRent.Reviews.Domain.Common;
using TrailRent.Reviews.Domain.Entities;
using TrailRent.Reviews.Domain.ValueObjects;
using Xunit;
namespace TrailRent.Reviews.Tests;
public sealed class ReviewTests
{
    [Fact] public void Rating_outside_range_throws_without_database() => Assert.Throws<DomainException>(() => new Review(new ProductReference(1, "Tent"), new Author("u1", "User"), 10, "Bad rating"));
    [Fact] public void Value_objects_compare_by_value() => Assert.Equal(new Author("u1", "User"), new Author("u1", "User"));
}
