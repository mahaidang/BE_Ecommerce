"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useState } from "react";
import { useUpdateProduct } from "../hooks";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";
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

export function EditProductDialog({ product }: { product: FormValues }) {
    const [open, setOpen] = useState(false);
    const { mutateAsync, isPending } = useUpdateProduct();

    const form = useForm<FormValues>({
        resolver: zodResolver(schema),
        defaultValues: product,
    });

    const onSubmit = async (data: FormValues) => {
        try {
            await mutateAsync({ dto: data });
            setOpen(false);
        } catch (err: any) {
            alert("Lỗi khi cập nhật sản phẩm");
        }
    };

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
                <Button variant="outline" size="sm">Sửa</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>Chỉnh sửa sản phẩm</DialogTitle>
                </DialogHeader>

                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                    <Input placeholder="Tên" {...form.register("name")} />
                    <Input placeholder="SKU" {...form.register("sku")} />
                    <Input placeholder="Slug" {...form.register("slug")} />
                    <Input
                        placeholder="Giá"
                        type="number"
                        {...form.register("price", { valueAsNumber: true })}
                    />
                    <Input placeholder="Đơn vị (VND)" {...form.register("currency")} />
                    <Input placeholder="Category ID" {...form.register("categoryId")} />

                    <div className="flex items-center justify-between">
                        <span>Kích hoạt</span>
                        <Switch
                            checked={form.watch("isActive")}
                            onCheckedChange={(val) => form.setValue("isActive", val)}
                        />
                    </div>

                    <div className="flex justify-end gap-2 pt-2">
                        <Button type="button" variant="outline" onClick={() => setOpen(false)}>
                            Hủy
                        </Button>
                        <Button type="submit" disabled={isPending}>
                            {isPending ? "Đang lưu..." : "Lưu"}
                        </Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}
