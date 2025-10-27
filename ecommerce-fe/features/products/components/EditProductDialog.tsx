"use client";

import { Button } from "@/components/ui/button";
import { useState } from "react";

import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";
import { TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Tabs } from "@radix-ui/react-tabs";
import { ProductForm } from "./ProductForm ";
import { ProductImagesGrid } from "./ProductImagesGrid";



export function EditProductDialog({ productId }: { productId: string }) {
    const [open, setOpen] = useState(false);


    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
                <Button variant="outline" size="sm">Sửa</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>Chỉnh sửa sản phẩm</DialogTitle>
                </DialogHeader>

                <Tabs defaultValue="info">
                    <TabsList className="mb-4">
                        <TabsTrigger value="info">Thông tin</TabsTrigger>
                        <TabsTrigger value="images">Hình ảnh</TabsTrigger>
                    </TabsList>

                    <TabsContent value="info">
                        <ProductForm productId={productId} onClose={() => setOpen(false)} />
                    </TabsContent>

                    <TabsContent value="images">
                        <ProductImagesGrid productId={productId} />

                    </TabsContent>
                </Tabs>
            </DialogContent>
        </Dialog>
    );
}
