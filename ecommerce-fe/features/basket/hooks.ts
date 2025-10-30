// // React hooks for customer basket/cart
// import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
// import { fetchBasket, addToBasket, removeFromBasket, updateBasketItem } from "./api";

// export function useBasket() {
//   return useQuery({
//     queryKey: ["basket"],
//     queryFn: fetchBasket,
//   });
// }

// export function useAddToBasket() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: addToBasket,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }

// export function useRemoveFromBasket() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: removeFromBasket,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }

// export function useUpdateBasketItem() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: updateBasketItem,
//     onSuccess: () => queryClient.invalidateQueries({ queryKey: ["basket"] }),
//   });
// }
