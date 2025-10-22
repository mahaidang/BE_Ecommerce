export interface Product {
  id: string;
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

export interface ProductPage {
  items: Product[];
  page: number;
  pageSize: number;
  total: number;
}
