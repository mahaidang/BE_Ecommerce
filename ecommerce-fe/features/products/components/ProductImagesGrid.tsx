"use client";

import { useState } from "react";
import { useProductImagesList, useSetMainImg } from "../hooks";
import { Trash2, Star } from "lucide-react";
import { Button } from "@/components/ui/button";

type Props = {
    productId: string;
};

export function ProductImagesGrid({ productId }: Props) {
    const { data: images, isLoading } = useProductImagesList(productId);
    const [selected, setSelected] = useState<string[]>([]);
    const setMainImg = useSetMainImg(productId);

    const toggleSelect = (id: string) => {
        setSelected((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
        );
    };

    const handleDelete = () => {
        if (selected.length === 0) return;
        if (!confirm(`Bạn có chắc muốn xóa ${selected.length} ảnh đã chọn không?`))
            return;
        console.log("🗑 Gửi API xóa:", selected);
        // TODO: Gọi API delete
        setSelected([]);
    };


    const handleSetMain = () => {
        if (selected.length !== 1) {
            alert("Chỉ được chọn 1 ảnh để đặt làm ảnh chính.");
            return;
        }
        setMainImg.mutate(selected[0], {
            onSuccess: () => setSelected([])
        });
    };

    // await setMainImg.mutateAsync(selected[0]);

    if (!images || images.length === 0)
        return (
            <div>
                <button className=" bg-gray-100 dark:bg-gray-800 p-3 rounded-md shadow-sm">Thêm ảnh</button>
                <div className="p-4 text-sm text-muted-foreground">
                    Không có ảnh cho sản phẩm này.
                </div>
            </div>
        );

    return (
        <div className="space-y-4">
            {/* Toolbar hiển thị khi có ảnh được chọn */}

            <div className="flex items-center justify-between bg-gray-100 dark:bg-gray-800 p-3 rounded-md shadow-sm">
                <div className="text-sm text-gray-700 dark:text-gray-200">
                    <button>Thêm ảnh</button>
                </div>
                {selected.length > 0 && (
                    <div className="flex gap-3">
                        <button
                            onClick={handleSetMain}
                            className="flex items-center gap-1 text-blue-600 hover:text-blue-800 dark:text-blue-300 dark:hover:text-blue-200"
                        >
                            Đặt ảnh chính
                        </button>
                        <button
                            onClick={handleDelete}
                            className="flex items-center gap-1 text-red-600 hover:text-red-800 dark:text-red-300 dark:hover:text-red-200"
                        >
                            Xóa ảnh
                        </button>
                    </div>
                )}

            </div>

            {/* Grid ảnh */}
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
                {images
                    .filter((i): i is { publicId: string; url: string; alt?: string; isMain?: boolean } => !!i.publicId)
                    .map((img) => {
                        const isSelected = selected.includes(img.publicId);

                        return (
                            <div
                                key={img.publicId}
                                className={`relative border rounded overflow-hidden transition cursor-pointer 
                ${isSelected ? "ring-2 ring-blue-500 dark:ring-blue-400" : "hover:ring-1 hover:ring-gray-300 dark:hover:ring-gray-600"}`}
                                onClick={() => toggleSelect(img.publicId)}
                            >
                                {/* Checkbox góc phải trên */}
                                <div
                                    className="absolute top-2 right-2 z-10"
                                    onClick={(e) => e.stopPropagation()}
                                >
                                    <input
                                        type="checkbox"
                                        checked={isSelected}
                                        onChange={() => toggleSelect(img.publicId)}
                                        className="w-5 h-5 accent-blue-600 dark:accent-blue-400 cursor-pointer"
                                    />
                                </div>

                                {/* Ảnh */}
                                <img
                                    src={img.url}
                                    alt={img.alt ?? "product image"}
                                    className="w-full h-36 object-cover"
                                    onError={(e) =>
                                        ((e.target as HTMLImageElement).src = "/placeholder.png")
                                    }
                                />
                            </div>
                        );
                    })}
            </div>
        </div>
    );
}
