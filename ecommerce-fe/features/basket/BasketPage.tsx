// "use client";
// import { useBasket, useRemoveFromBasket, useUpdateBasketItem } from "./hooks";
// import BasketList from "./components/BasketList";

// export default function BasketPage() {
//   const basket = useBasket();
//   const remove = useRemoveFromBasket();
//   const update = useUpdateBasketItem();

//   return (
//     <div className="max-w-2xl mx-auto py-8">
//       <h1 className="text-2xl font-bold mb-6">Giỏ hàng của bạn</h1>
//       <BasketList
//         items={basket.data || []}
//         onRemove={id => remove.mutate(id)}
//         onUpdate={(id, qty) => update.mutate({ productId: id, quantity: qty })}
//       />
//       {/* TODO: Thêm tổng tiền, nút thanh toán, v.v. */}
//     </div>
//   );
// }
