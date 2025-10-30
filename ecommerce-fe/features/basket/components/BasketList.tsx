"use client";
import { BasketItem } from "../types";

interface BasketListProps {
  items: BasketItem[];
  onRemove: (productId: string) => void;
  onUpdate: (productId: string, quantity: number) => void;
}

export default function BasketList({ items, onRemove, onUpdate }: BasketListProps) {
  return (
    <div>
      {items.length === 0 ? (
        <div className="text-center text-gray-400 py-8">Giỏ hàng trống</div>
      ) : (
        <ul className="divide-y">
          {items.map((item) => (
            <li key={item.product.id} className="flex items-center gap-4 py-3">
              <img src={item.product.images?.[0]?.url} alt={item.product.name} className="w-14 h-14 object-contain rounded border" />
              <div className="flex-1">
                <div className="font-medium">{item.product.name}</div>
                <div className="text-sm text-gray-500">Số lượng: {item.quantity}</div>
              </div>
              <input
                type="number"
                min={1}
                value={item.quantity}
                onChange={e => onUpdate(item.product.id, Number(e.target.value))}
                className="w-16 border rounded px-2 py-1 text-center"
              />
              <button onClick={() => onRemove(item.product.id)} className="ml-2 text-red-500 hover:underline text-xs">Xóa</button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
