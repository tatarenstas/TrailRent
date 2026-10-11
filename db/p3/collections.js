const database = db.getSiblingDB("TrailRent_CommunityDB");

["equipment_reviews", "trip_discussions", "trail_clubs"].forEach((name) => {
  if (!database.getCollectionNames().includes(name)) {
    database.createCollection(name);
  }
});

database.equipment_reviews.createIndex({ equipmentId: 1, createdAt: -1 });
database.equipment_reviews.createIndex({ "author.userId": 1 });
database.trip_discussions.createIndex({ tags: 1, lastMessageAt: -1 });
database.trail_clubs.createIndex({ city: 1 });

print("Collections prepared: equipment_reviews, trip_discussions, trail_clubs");
