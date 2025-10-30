import { Product } from "@/features/products/types";

export interface BasketItem {
  product: Product;
  quantity: number;
}
