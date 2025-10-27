"use client"

import { useProductImagesList } from "../hooks";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { productApi } from "../api";
import { ProductImage } from "../types";

type Props = {
        productId: string;
        onClose?: () => void;
};

export function ProductImagesGrid({productId, onClose}: Props) {
        const { data: images, isLoading } = useProductImagesList(productId);
        const queryClient = useQueryClient();

        const deleteMutation = useMutation({
                mutationFn: (publicId: string) => productApi.deleteImage(productId, publicId),
                onSuccess: () => {
                        // refresh product images and product list
                        queryClient.invalidateQueries({ queryKey: ["product", productId] });
                        queryClient.invalidateQueries({ queryKey: ["products"] });
                },
        });

        const handleDelete = async (img: ProductImage) => {
                if (!confirm("Xóa ảnh này?")) return;
                try {
                        await deleteMutation.mutateAsync(img.publicId);
                } catch (err) {
                        // keep simple: alert on error
                        alert("Xóa ảnh thất bại");
                }
        };

        // marking an existing image as main requires a dedicated API endpoint.
        // If your backend supports it, replace the placeholder below with the proper call.
        const handleSetMain = (img: ProductImage) => {
                alert("Chức năng đặt làm ảnh chính chưa được triển khai trên client. Cần endpoint trên backend.");
        };

        if (isLoading) {
                return <div className="p-4">Đang tải ảnh...</div>;
        }

        if (!images || images.length === 0) {
                return <div className="p-4 text-sm text-muted-foreground">Không có ảnh cho sản phẩm này.</div>;
        }

        return (
                <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
                        {images.map((img) => (
                                <div key={img.publicId} className="relative border rounded overflow-hidden">
                                        <img
                                                src={img.url}
                                                alt={img.alt ?? "product image"}
                                                className="w-full h-36 object-cover"
                                                onError={(e) => {
                                                        (e.target as HTMLImageElement).src = "/placeholder.png";
                                                }}
                                        />

                                        {img.isMain && (
                                                <span className="absolute top-2 left-2 bg-blue-600 text-white text-xs px-2 py-1 rounded">Chính</span>
                                        )}

                                        <div className="absolute right-2 top-2 flex flex-col gap-2">
                                                {!img.isMain && (
                                                        <button
                                                                type="button"
                                                                onClick={() => handleSetMain(img)}
                                                                className="bg-white bg-opacity-90 text-sm px-2 py-1 rounded shadow"
                                                        >
                                                                Đặt chính
                                                        </button>
                                                )}

                                                                                        <button
                                                                                                type="button"
                                                                                                onClick={() => handleDelete(img)}
                                                                                                      disabled={deleteMutation.status === "pending"}
                                                                                                className="bg-red-600 text-white text-sm px-2 py-1 rounded shadow"
                                                                                        >
                                                        Xóa
                                                </button>
                                        </div>
                                </div>
                        ))}
                </div>
        );
}