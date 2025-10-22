"use client";

import { useState } from "react";
import { useProducts } from "@/features/products/hooks";
import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

export default function ProductsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { data, isLoading, isError } = useProducts(page, pageSize);

  if (isError)
    return <div className="p-8 text-red-500">Lỗi tải danh sách sản phẩm</div>;

  return (
    <main className="p-6 space-y-4">
      <h1 className="text-2xl font-semibold mb-4">Danh sách sản phẩm</h1>

      <Card className="p-4">
        {isLoading ? (
          <div className="space-y-3">
            {Array.from({ length: 5 }).map((_, i) => (
              <Skeleton key={i} className="h-8 w-full" />
            ))}
          </div>
        ) : (
          <table className="w-full border-collapse">
            <thead>
              <tr className="border-b text-left">
                <th className="p-2">Tên</th>
                <th className="p-2">SKU</th>
                <th className="p-2">Giá</th>
                <th className="p-2">Trạng thái</th>
                <th className="p-2">Ngày tạo</th>
              </tr>
            </thead>
            <tbody>
              {data?.map((p) => (
                <tr key={p._id} className="border-b hover:bg-muted/20">
                  <td className="p-2">{p.name}</td>
                  <td className="p-2 text-sm text-muted-foreground">{p.sku}</td>
                  <td className="p-2">
                    {p.price.toLocaleString("vi-VN")} {p.currency}
                  </td>
                  <td className="p-2">
                    <span
                      className={`px-2 py-1 text-xs rounded ${
                        p.isActive
                          ? "bg-green-100 text-green-700"
                          : "bg-gray-200 text-gray-600"
                      }`}
                    >
                      {p.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td className="p-2 text-sm">
                    {new Date(p.createdAtUtc).toLocaleDateString("vi-VN")}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>
    </main>
  );
}
