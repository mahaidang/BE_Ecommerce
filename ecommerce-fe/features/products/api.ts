import api from "@/lib/api";
import { CreateProductDto, Product, ProductFilter, ProductPage, UpdateProductDto } from "./types";

export const productApi = {
  search: async (filters: ProductFilter): Promise<ProductPage> => {
    const res = await api.get("api/product/products", { params: filters });
    return res.data;
  },

  create: async (dto: CreateProductDto) : Promise<Product> => {
    const res = await api.post("api/product/products", dto);
    return res.data;
  },
  update: async (dto: UpdateProductDto, etag?: string): Promise<Product> => {
    const headers = etag ? { "If-Match": etag } : {};
    const res = await api.put(`api/product/products/${dto.id}`, dto, { headers });
    return res.data;
  },
  delete: async (id: string) : Promise<void> => {
    const res = await api.delete(`api/product/products/${id}`);
    return res.data;
  },
  uploadImage: async (productId: string, file: File, isMain: boolean) => {
    const form = new FormData();
    form.append("file", file);
    form.append("isMain", String(isMain));

    const res = await api.post(`api/product/products/${productId}/images`, form, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    return res.data;
  },

  deleteImage: async (productId: string, publicId: string) => {
    await api.delete(`api/product/products/${productId}/images/${publicId}`);
  },
};
