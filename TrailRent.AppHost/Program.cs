var builder = DistributedApplication.CreateBuilder(args);

var bookingDb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("BookingDb");

builder.AddProject<Projects.TrailRent_Api>("trailrent-api")
    .WithReference(bookingDb);

builder.Build().Run();
