"use client";

import { useState } from "react";
import { useProducts } from "@/features/products/hooks";
import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";

export default function ProductsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { data, isLoading, isError } = useProducts(page, pageSize);

  if (isError) return <div className="p-8 text-red-500">Lỗi tải dữ liệu sản phẩm</div>;

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
          <>
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
                {data?.items.map((p) => (
                  <tr key={p.id} className="border-b hover:bg-muted/30">
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

            {/* Pagination */}
            <div className="flex justify-end mt-4 gap-2">
              <Button
                variant="outline"
                size="sm"
                disabled={page === 1}
                onClick={() => setPage((p) => Math.max(p - 1, 1))}
              >
                Trước
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={page * pageSize >= (data?.total ?? 0)}
                onClick={() => setPage((p) => p + 1)}
              >
                Sau
              </Button>
            </div>
          </>
        )}
      </Card>
    </main>
  );
}
