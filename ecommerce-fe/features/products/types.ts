export interface Product {
  _id: string;
  sku: string;
  name: string;
  slug: string;
  categoryId: string;
  price: number;
  currency: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}
