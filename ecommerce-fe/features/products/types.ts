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

export interface ProductFilter {
  page?: number;
  pageSize?: number;
  keyword?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
}