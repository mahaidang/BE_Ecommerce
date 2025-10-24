"use client";

import { useState } from "react";
import { productApi } from "../api";
import { Button } from "@/components/ui/button";
import { Product } from "../types";

export function ProductImageUpload({
  product,
  onUploaded,
}: {
  product: Product;
  onUploaded?: () => void;
}) {
  const [files, setFiles] = useState<File[]>([]);
  const [previews, setPreviews] = useState<string[]>([]);
  const [uploading, setUploading] = useState(false);

  const handleSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files) return;
    const selected = Array.from(e.target.files);
    setFiles(selected);
    setPreviews(selected.map((f) => URL.createObjectURL(f)));
  };

  const handleUpload = async () => {
    if (!files.length) return;
    setUploading(true);
    try {
      for (let i = 0; i < files.length; i++) {
        await productApi.uploadImage(product.id, files[i], i === 0); // ảnh đầu tiên = isMain
      }
      onUploaded?.();
      setFiles([]);
      setPreviews([]);
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="space-y-3">
      <input
        type="file"
        multiple
        accept="image/*"
        onChange={handleSelect}
        className="block w-full text-sm text-gray-600"
      />

      {previews.length > 0 && (
        <div className="flex gap-2 flex-wrap">
          {previews.map((src, i) => (
            <img
              key={i}
              src={src}
              alt="preview"
              className="w-20 h-20 object-cover rounded-md border"
            />
          ))}
        </div>
      )}

      <Button
        variant="secondary"
        disabled={!files.length || uploading}
        onClick={handleUpload}
      >
        {uploading ? "Đang tải..." : "Tải ảnh lên Cloudinary"}
      </Button>
    </div>
  );
}
