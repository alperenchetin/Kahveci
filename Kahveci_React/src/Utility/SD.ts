export enum SD_Roles {
  ADMIN = "admin",
  CUTOMER = "customer",
}

export enum SD_Status {
  PENDING = "Pending",
  CONFIRMED = "Confirmed",
  BEING_COOKED = "Being Cooked",
  READY_FOR_PICKUP = "Ready for Pickup",
  COMPLETED = "Completed",
  CANCELLED = "Cancelled",
}

export enum SD_Categories {
  APPETIZER = "Sıcak",
  ENTREE = "Soğuk",
  DESSERT = "Kahve Ekipmanları",
  BEVERAGES = "Yiyecekler",
}

export enum SD_SortTypes {
  PRICE_LOW_HIGH = "En düşük fiyat",
  PRICE_HIGH_LOW = "En yüksek fiyat",
  NAME_A_Z = "A'dan Z'ye",
  NAME_Z_A = "Z'den A'ya",
}
