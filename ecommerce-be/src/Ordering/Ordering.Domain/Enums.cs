namespace Ordering.Domain;

public enum OrderStatus : byte { Pending = 0, Confirmed = 1, Paid = 2 , Shipped = 3, Completed = 4, Cancelled = 5}
