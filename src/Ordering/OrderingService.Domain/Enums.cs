using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain;

public enum OrderStatus : byte { Pending = 0, Confirmed = 1, Paid = 2 , Shipped = 3, Completed = 4, Cancelled = 5}
