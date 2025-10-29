import api from "@/lib/api";
import { CreateProductDto, ListImageDto, Product, ProductFilter, ProductPage, UpdateProductDto } from "./types";

export const productApi = {
  //list
  search: async (filters: ProductFilter): Promise<ProductPage> => {
    const res = await api.get("api/product/products", { params: filters });
    return res.data;
  },

  //detail
  detail: async (id: string) : Promise<Product> => {
    const res = await api.get(`api/product/products/${id}`);
    return res.data;
  },
  
  //full
  full: async (id: string) : Promise<Product> => {
    const res = await api.get(`api/product/products/${id}/full`);
    return res.data;
  },

  //list img
  images: async (id: string) : Promise<ListImageDto> => {
    const res = await api.get(`api/product/products/${id}/images`);
    return res.data;
  },

  //create prod
  create: async (dto: CreateProductDto) : Promise<Product> => {
    const res = await api.post("api/product/products", dto);
    return res.data;
  },

  //edit prod
  update: async (dto: UpdateProductDto, etag?: string): Promise<Product> => {
    const headers = etag ? { "If-Match": etag } : {};
    const res = await api.put(`api/product/products/${dto.id}`, dto, { headers });
    return res.data;
  },

  //del prod
  delete: async (id: string) : Promise<void> => {
    const res = await api.delete(`api/product/products/${id}`);
    return res.data;
  },

  //up img
  uploadImage: async (productId: string, file: File, isMain: boolean) => {
    const form = new FormData();
    form.append("file", file);
    form.append("isMain", String(isMain));

    const res = await api.post(`api/product/products/${productId}/images`, form, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    return res.data;
  },

  //del img
  deleteImage: async (productId: string, publicId: string) => {
    await api.delete(`api/product/products/${productId}/images/${publicId}`);
  },

  //set main img
  setMainImg: async (productId: string, publicId: string) => {
    const encodedId = encodeURIComponent(publicId);
    await api.post(`api/product/products/${productId}/images/${encodedId}/main`);
  }
};
