import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productApi } from "./api";
import { CreateProductDto, ProductFilter, UpdateProductDto } from "./types";

export function useProducts(filters: ProductFilter) {
  return useQuery({
    queryKey: ["products", filters],
    queryFn: () => productApi.search(filters),
  });

}

export function useProduct(id?: string) {
  return useQuery({
    queryKey: ["product", id],
    queryFn: () => productApi.detail(id as string),
    enabled: !!id,
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

export function useProductImagesList(productId?: string) {
  return useQuery({
    queryKey: ["product", productId, "images"],
    queryFn: async () => {
      const product = await productApi.images(productId as string);
      return product.images ?? [];
    },
    enabled: !!productId,
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ dto, etag }: { dto: UpdateProductDto; etag?: string }) =>
      productApi.update(dto, etag),
    onMutate: async ({ dto }) => {
      // Optimistic update: cập nhật tạm UI
      await queryClient.cancelQueries({ queryKey: ["products"] });
      const previousData = queryClient.getQueryData<any>(["products"]);
      queryClient.setQueryData(["products"], (old: any) => {
        if (!old) return old;
        return {
          ...old,
          items: old.items.map((p: any) => (p.id === dto.id ? { ...p, ...dto } : p)),
        };
      });
      return { previousData };
    },
    onError: (_err, _dto, context) => {
      // Rollback nếu lỗi
      queryClient.setQueryData(["products"], context?.previousData);
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => productApi.delete(id),
    onMutate: async (id) => {
      // Optimistic update – xóa tạm trên UI
      await queryClient.cancelQueries({ queryKey: ["products"] });
      const previousData = queryClient.getQueryData<any>(["products"]);
      queryClient.setQueryData(["products"], (old: any) => {
        if (!old) return old;
        return {
          ...old,
          items: old.items.filter((p: any) => p.id !== id),
        };
      });
      return { previousData };
    },
    onError: (_err, _id, context) => {
      // Rollback nếu lỗi
      queryClient.setQueryData(["products"], context?.previousData);
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
}

