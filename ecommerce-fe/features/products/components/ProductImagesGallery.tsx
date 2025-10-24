"use client";

import { productApi } from "../api";
import { Product } from "../types";
import { Button } from "@/components/ui/button";
import { useState } from "react";

export function ProductImagesGallery({ product, onDeleted }: { product: Product; onDeleted?: () => void }) {
  const [deleting, setDeleting] = useState<string | null>(null);

  if (!product.images?.length) return <p className="text-gray-500">Chưa có ảnh</p>;

  const handleDelete = async (publicId: string) => {
    if (!confirm("Xoá ảnh này?")) return;
    setDeleting(publicId);
    await productApi.deleteImage(product.id, publicId);
    onDeleted?.();
    setDeleting(null);
  };

  return (
    <div className="flex flex-wrap gap-3 mt-2">
      {product.images.map((img) => (
        <div key={img.publicId} className="relative">
          <img
            src={img.url}
            alt={img.alt || ""}
            className={`w-24 h-24 rounded border object-cover ${
              img.isMain ? "ring-2 ring-blue-500" : ""
            }`}
          />
          <Button
            size="icon"
            variant="destructive"
            className="absolute top-0 right-0 w-6 h-6"
            onClick={() => handleDelete(img.publicId)}
            disabled={deleting === img.publicId}
          >
            ×
          </Button>
        </div>
      ))}
    </div>
  );
}
