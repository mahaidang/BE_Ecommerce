import { useState } from "react";
import { z } from "zod";
import { useCreateProduct } from "../hooks";
import { zodResolver } from "@hookform/resolvers/zod";
import {
    Dialog,
    DialogTrigger,
    DialogContent,
    DialogHeader,
    DialogTitle
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { useForm } from "react-hook-form";


const schema = z.object({
    name: z.string().min(1, "Tên không được trống"),
    sku: z.string().min(1, "SKU không được trống"),
    slug: z.string().min(1, "SKU không được trống"),
    price: z.number().positive("Giá phải lớn hơn 0"),
    currency: z.string().min(3, "Đơn vị là 3 ký tự").max(3, "Đơn vị là 3 ký tự"),
    categoryId: z.string().uuid("CategoryId không hợp lệ"),
    isActive: z.boolean(),
});

type FormValues = z.infer<typeof schema>;

export function CreateProductDialog() {
    const [open, setOpen] = useState(false);
    const { mutateAsync, isPending } = useCreateProduct();

    const form = useForm<FormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            name: "",
            sku: "",
            slug: "",
            price: 0,
            currency: "VND",
            categoryId: "",
            isActive: true,
        },
    });

    const onSubmit = async (data: FormValues) => {
        try {
            await mutateAsync(data);
            setOpen(false);
            form.reset();
        } catch (err: any) {
            if (err.response?.status === 422) {
                alert("Dữ liệu không hợp lệ (422)");
            } else {
                alert("Lỗi khi tạo sản phẩm");
            }
        }
    };

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
                <Button>+ Thêm sản phẩm</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>Tạo sản phẩm mới</DialogTitle>
                </DialogHeader>

                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                    <div>
                        <label className="text-sm font-medium">Tên sản phẩm</label>
                        <Input {...form.register("name")} />
                        {form.formState.errors.name && (
                            <p className="text-red-500 text-sm">
                                {form.formState.errors.name.message}
                            </p>
                        )}
                    </div>

                    <div>
                        <label className="text-sm font-medium">Sku</label>
                        <Input {...form.register("sku")} />
                    </div>
                    <div>
                        <label className="text-sm font-medium">Slug</label>
                        <Input {...form.register("slug")} />
                    </div>

                    <div>
                        <label className="text-sm font-medium">Giá</label>
                        <Input
                            type="number"
                            {...form.register("price", { valueAsNumber: true })}
                        />
                    </div>

                    <div>
                        <label className="text-sm font-medium">Category ID</label>
                        <Input {...form.register("categoryId")} />
                    </div>

                    <div className="flex items-center justify-between">
                        <span className="text-sm font-medium">Kích hoạt</span>
                        <Switch
                            checked={form.watch("isActive")}
                            onCheckedChange={(val) => form.setValue("isActive", val)}
                        />
                    </div>

                    <div className="flex justify-end gap-2 pt-2">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => setOpen(false)}
                        >
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