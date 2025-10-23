import { useQuery } from "@tanstack/react-query";
import { productApi } from "./api";
import { ProductFilter } from "./types";

export function useProducts(filters: ProductFilter) {
  return useQuery({
    queryKey: ["products", filters],
    queryFn: () => productApi.search(filters),
  });
}
