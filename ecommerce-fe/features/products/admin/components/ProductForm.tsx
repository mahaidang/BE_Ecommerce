"use client";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useProduct, useUpdateProduct } from "../../hooks";

import { Switch } from "@/components/ui/switch";

const schema = z.object({
    id: z.string(),
    name: z.string().min(1, "Tên không được trống"),
    sku: z.string().min(1, "SKU không được trống"),
    slug: z.string().min(1, "SKU không được trống"),
    price: z.number().positive("Giá phải lớn hơn 0"),
    currency: z.string().min(3, "Đơn vị là 3 ký tự").max(3, "Đơn vị là 3 ký tự"),
    categoryId: z.string().uuid("CategoryId không hợp lệ"),
    isActive: z.boolean(),
});

type FormValues = z.infer<typeof schema>;

export function ProductForm({ productId, onClose }: { productId: string; onClose?: () => void }) {
    const { data: product, isLoading } = useProduct(productId);
    const { mutateAsync, isPending } = useUpdateProduct();

        const form = useForm<FormValues>({
                resolver: zodResolver(schema),
                defaultValues: {
                        id: "",
                        name: "",
                        sku: "",
                        slug: "",
                        price: 0,
                        currency: "VND",
                        categoryId: "",
                        isActive: true,
                },
        });

        useEffect(() => {
            if (product) {
                form.reset({
                    id: product.id,
                    name: product.name,
                    sku: product.sku,
                    slug: product.slug ?? "",
                    price: product.price ?? 0,
                    currency: product.currency ?? "VND",
                    categoryId: product.categoryId ?? "",
                    isActive: !!product.isActive,
                });
            }
        }, [product]);

    const onSubmit = async (data: FormValues) => {
        try {
            await mutateAsync({ dto: data });
            // close surrounding dialog if provided
            onClose?.();
        } catch (err: any) {
            alert("Lỗi khi cập nhật sản phẩm");
        }
    };

    return (
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            {isLoading && <div className="text-sm text-muted-foreground">Đang tải dữ liệu...</div>}

            <div>
                <label className="text-sm font-medium block mb-1">Tên sản phẩm</label>
                <Input placeholder="Tên" {...form.register("name")} />
                {form.formState.errors.name && (
                    <p className="text-red-500 text-sm">{form.formState.errors.name.message}</p>
                )}
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">Mã hàng</label>
                <Input placeholder="SKU" {...form.register("sku")} />
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">URL slug</label>
                <Input placeholder="Slug" {...form.register("slug")} />
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">Giá</label>
                <Input
                    placeholder="Giá"
                    type="number"
                    {...form.register("price", { valueAsNumber: true })}
                />
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">Đơn vị</label>
                <Input placeholder="Đơn vị (VND)" {...form.register("currency")} />
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">Category ID</label>
                <Input placeholder="Category ID" {...form.register("categoryId")} />
            </div>

            <div>
                <label className="text-sm font-medium block mb-1">Kích hoạt</label>
                <div className="flex items-center">
                    <Switch
                        checked={form.watch("isActive")}
                        onCheckedChange={(val) => form.setValue("isActive", val)}
                    />
                </div>
            </div>


            <div className="flex justify-end gap-2 pt-2">
                <Button type="button" variant="outline" onClick={() => { form.reset(); onClose?.(); }}>
                    Hủy
                </Button>
                <Button type="submit" disabled={isPending}>
                    {isPending ? "Đang lưu..." : "Lưu"}
                </Button>
            </div>
        </form>
    );
}
