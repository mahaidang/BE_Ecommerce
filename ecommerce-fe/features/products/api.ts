import api from "@/lib/api";
import { CreateProductDto, Product, ProductFilter, ProductPage } from "./types";

export const productApi = {
  search: async (filters: ProductFilter): Promise<ProductPage> => {
    const res = await api.get("api/product/Products", { params: filters });
    return res.data;
  },

  create: async (dto: CreateProductDto) : Promise<Product> => {
    const res = await api.post("api/product/Products", dto);
    return res.data;
  },
};
