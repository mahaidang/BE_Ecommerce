import api from "@/lib/api";
import { ProductFilter, ProductPage } from "./types";

export const productApi = {
  search: async (filters: ProductFilter): Promise<ProductPage> => {
    const res = await api.get("api/product/Products", { params: filters });
    return res.data;
  },
};
