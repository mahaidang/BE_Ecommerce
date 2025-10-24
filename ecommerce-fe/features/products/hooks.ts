import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productApi } from "./api";
import { CreateProductDto, ProductFilter } from "./types";

export function useProducts(filters: ProductFilter) {
  return useQuery({
    queryKey: ["products", filters],
    queryFn: () => productApi.search(filters),
  });

}

export function useCreateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (dto: CreateProductDto) => productApi.create(dto),
    onSuccess: () => {
      // Khi thêm mới thành công → refetch list sản phẩm
      queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
}
