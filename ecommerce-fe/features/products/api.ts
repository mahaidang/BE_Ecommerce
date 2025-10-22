import api from "@/lib/api";
import { ProductPage } from "./types";

export const productApi = {
  getAll: async (page: number = 1, pageSize: number = 20): Promise<ProductPage> => {
    const res = await api.get("api/product/Products", { params: { page, pageSize } });
    return res.data;
  },
};
