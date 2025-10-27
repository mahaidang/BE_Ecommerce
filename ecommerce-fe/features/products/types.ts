export interface ProductImage {
  url: string;
  publicId: string;
  isMain: boolean;
  alt?: string;
}

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
   images?: ProductImage[];
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

export interface CreateProductDto {
  sku: string;
  name: string;
  slug: string;
  categoryId?: string;
  price: number;
  currency: string;
  isActive?: boolean;
}

export interface UpdateProductDto {
  id?: string;
  sku: string;
  name: string;
  slug: string;
  categoryId?: string;
  price: number;
  currency: string;
  isActive?: boolean;
}

export interface ImgDto {
  url?: string;
  publicId?: string;
}

export interface ListImageDto {
  images?: ImgDto[];
}