// API functions for customer basket/cart
import { Product } from "@/features/products/types";

export interface BasketItem {
  product: Product;
  quantity: number;
}

export async function fetchBasket() {
  // TODO: Replace with real API call
  return [] as BasketItem[];
}

export async function addToBasket(productId: string, quantity: number) {
  // TODO: Replace with real API call
  return { success: true };
}

export async function removeFromBasket(productId: string) {
  // TODO: Replace with real API call
  return { success: true };
}

export async function updateBasketItem(productId: string, quantity: number) {
  // TODO: Replace with real API call
  return { success: true };
}
