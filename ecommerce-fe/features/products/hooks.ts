import { useQuery } from "@tanstack/react-query";
import { productApi } from "./api";

export function useProducts(page: number, pageSize: number) {
  return useQuery({
    queryKey: ["products", page, pageSize],
    queryFn: () => productApi.getAll(page, pageSize),
  });
}
