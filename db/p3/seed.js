const database = db.getSiblingDB("TrailRent_CommunityDB");

function upsert(collection, document) {
  collection.replaceOne({ _id: document._id }, document, { upsert: true });
}

upsert(database.equipment_reviews, {
  _id: ObjectId("66f0d1000000000000000001"),
  equipmentId: 1,
  equipmentName: "Намет Alpine Duo 2P",
  author: { userId: 1, displayName: "Олена Коваль" },
  rating: 5,
  text: "Витримав дощ на Свидовці, швидко ставиться навіть удвох.",
  trip: { routeName: "Свидовецький хребет", days: 3 },
  comments: [{
    author: { userId: 2, displayName: "Максим Бойко" },
    text: "А як поводяться тамбури під сильним вітром?",
    createdAt: ISODate("2026-10-05T09:10:00Z")
  }],
  discussionId: ObjectId("66f0d2000000000000000001"),
  createdAt: ISODate("2026-10-04T18:30:00Z")
});

upsert(database.equipment_reviews, {
  _id: ObjectId("66f0d1000000000000000002"),
  equipmentId: 3,
  equipmentName: "Спальник FrostLine -5°C",
  author: { userId: 2, displayName: "Максим Бойко" },
  rating: 4,
  text: "Теплий і компактний, але для високого зросту хотілося б трохи більше довжини.",
  pros: ["теплий", "легкий"],
  cons: ["вузький у плечах"],
  createdAt: ISODate("2026-10-06T20:15:00Z")
});

upsert(database.equipment_reviews, {
  _id: ObjectId("66f0d1000000000000000003"),
  equipmentId: 4,
  equipmentName: "Пальник FireTrail Compact",
  author: { userId: 3, displayName: "Софія Мельник" },
  rating: 5,
  text: "Закип'ятив літр води дуже швидко. Зручно пакується в казанок.",
  stove: { fuel: "ізобутан", windShieldUsed: true },
  createdAt: ISODate("2026-10-08T07:45:00Z")
});

upsert(database.trip_discussions, {
  _id: ObjectId("66f0d2000000000000000001"),
  title: "Що взяти на осінній Свидовець на три дні?",
  category: "packing",
  createdBy: { userId: 1, displayName: "Олена Коваль" },
  equipmentIds: [1, 3, 5],
  tags: ["Свидовець", "осінь", "список спорядження"],
  messageCount: 4,
  lastMessageAt: ISODate("2026-10-06T18:20:00Z"),
  pinnedMessages: [{
    author: { userId: 1, displayName: "Олена Коваль" },
    text: "Візьміть запасний комплект сухого одягу та гермомішки.",
    createdAt: ISODate("2026-10-04T18:45:00Z")
  }],
  createdAt: ISODate("2026-10-04T18:35:00Z")
});

upsert(database.trail_clubs, {
  _id: ObjectId("66f0d3000000000000000001"),
  name: "Буковинський трейл-клуб",
  city: "Чернівці",
  description: "Спільні походи Карпатами для початківців і досвідчених туристів.",
  locations: [{
    title: "Зустріч біля вокзалу",
    address: "вул. Гагаріна, 38, Чернівці",
    geo: { type: "Point", coordinates: [25.936, 48.259] }
  }],
  events: [{
    date: ISODate("2026-10-18T07:00:00Z"),
    theme: "Одноденний маршрут на Цецино", seats: 18
  }],
  contacts: { telegram: "@bukovyna_trail" }
});

upsert(database.trail_clubs, {
  _id: ObjectId("66f0d3000000000000000002"),
  name: "Міські мандрівники Львова",
  city: "Львів",
  locations: [{
    title: "Старт від Стрийського парку",
    address: "вул. Стрийська, Львів",
    geo: { type: "Point", coordinates: [24.022, 49.825] }
  }],
  membersCount: 57,
  contacts: { instagram: "lviv_trail_weekend", email: "hello@lvivtrail.example" }
});

print("equipment_reviews: " + database.equipment_reviews.countDocuments() +
      ", trip_discussions: " + database.trip_discussions.countDocuments() +
      ", trail_clubs: " + database.trail_clubs.countDocuments());
