import api from "@/lib/api";
import { Product } from "./types";

export const productApi = {
  getAll: async (page: number = 1, pageSize: number = 10): Promise<Product[]> => {
    const res = await api.get("api/products", { params: { page, pageSize } });
    return res.data;
  },
};
