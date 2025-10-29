"use client";
"use client";
import React, { useState } from "react";
import { useProductFull } from "../hooks";
import { useParams } from "next/navigation";

// const mockProduct = {
//     id: "1",
//     name: "Áo thun nam basic",
//     price: 199000,
//     currency: "VND",
//     description: "Áo thun nam chất liệu cotton, thoáng mát, phù hợp mặc hàng ngày.",
//     images: [
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761713334/ecommerce/products/download_1_cmzvh3.jpg", alt: "Áo thun nam" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761705879/ecommerce/products/download_3_qpus3a.jpg", alt: "Áo thun nam 2" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761545402/ecommerce/products/571191660_1085337023500359_1414476991803960965_n_sh5vox.jpg", alt: "Áo thun nam 3" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761545302/ecommerce/products/images_ja5anl.jpg", alt: "Áo thun nam 4" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761534857/ecommerce/products/download_herec1.jpg", alt: "Áo thun nam 5" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761534857/ecommerce/products/download_herec1.jpg", alt: "Áo thun nam 5" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761534857/ecommerce/products/download_herec1.jpg", alt: "Áo thun nam 5" },
//         { url: "https://res.cloudinary.com/dt3k9eyfz/image/upload/v1761534857/ecommerce/products/download_herec1.jpg", alt: "Áo thun nam 5" },
//     ],
//     category: "Thời trang nam",
//     sku: "TSHIRT001",
//     isActive: true,
//     createdAtUtc: "2025-10-29T10:00:00Z",
// };

const formatPrice = (price: number) =>
    new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(price);


const COLOR_OPTIONS = [
    { name: "Đỏ", value: "red", class: "bg-red-500" },
    { name: "Xanh", value: "blue", class: "bg-blue-500" },
    { name: "Đen", value: "black", class: "bg-black" },
    { name: "Trắng", value: "white", class: "bg-white border border-gray-300" },
];

const CustomerProductDetail = () => {
    const params = useParams();
    const productId = params?.id as string;
    const product = useProductFull(productId);
    const [showColorModal, setShowColorModal] = useState(false);
    const [actionType, setActionType] = useState<"cart" | "buy" | null>(null);
    const [selectedColor, setSelectedColor] = useState<string>("");
    const [quantity, setQuantity] = useState<number>(1);
    const [mainImage, setMainImage] = useState(
        product.data?.images?.[0]?.url ?? "placeholder.png"
    );


    // 👉 state cho carousel
    const [startIndex, setStartIndex] = useState(0);
    const visibleImages = product.data?.images?.slice(startIndex, startIndex + 3);

    const next = () =>
        setStartIndex((prev) =>
            Math.min(prev + 3, (product.data?.images?.length ?? 0) - 3)
        );
    const prev = () =>
        setStartIndex((prev) =>
            Math.max(prev - 3, 0)
        );

    const handleChooseColor = (type: "cart" | "buy") => {
        setActionType(type);
        setShowColorModal(true);
        setQuantity(1);
    };

    const handleConfirm = () => {
        setShowColorModal(false);
        if (actionType === "cart") {
            // TODO: Thêm vào giỏ hàng với màu selectedColor và quantity
            alert(`Đã thêm vào giỏ hàng với màu: ${selectedColor}, số lượng: ${quantity}`);
        } else if (actionType === "buy") {
            // TODO: Xử lý mua ngay với màu selectedColor và quantity
            alert(`Mua ngay với màu: ${selectedColor}, số lượng: ${quantity}`);
        }
        setSelectedColor("");
        setActionType(null);
        setQuantity(1);
    };

    return (
        <div className="max-w-3xl mx-auto px-4 py-8 bg-white dark:bg-neutral-900 rounded-lg shadow">
            <h1 className="text-2xl font-bold mb-4 text-gray-900 dark:text-gray-100">{product.data?.name}</h1>
            <div className="flex flex-col md:flex-row gap-8">
                {/* 👉 Image carousel */}
                <div className="flex-shrink-0 w-full md:w-80">
                    {/* Ảnh chính */}
                    <div className="relative p-5">
                        <img
                            src={mainImage}
                            alt={product.data?.name}
                            className="w-full h-72 object-contain rounded-lg border border-gray-200 dark:border-neutral-700 bg-gray-100 dark:bg-neutral-800 transition-all duration-300"
                        />
                    </div>

                    {/* Carousel thumbnails (3 ảnh mỗi lần) */}
                    <div className="mt-3 flex items-center justify-center gap-2">
                        <button
                            onClick={prev}
                            disabled={startIndex === 0}
                            className="px-3 py-1 border rounded disabled:opacity-50"
                        >
                            ←
                        </button>
                        <div className="flex gap-2">
                            {visibleImages?.map((img, i) => (
                                <img
                                    key={i}
                                    src={img.url}
                                    alt={img.alt}
                                    onClick={() => setMainImage(img.url)}

                                    className="w-15 h-15 object-cover rounded border border-gray-200 dark:border-neutral-700 bg-gray-100 dark:bg-neutral-800"
                                />
                            ))}
                        </div>
                        <button
                            onClick={next}
                            disabled={startIndex + 3 >= (product.data?.images?.length ?? 0)}
                            className="px-3 py-1 border rounded disabled:opacity-50"
                        >
                            →
                        </button>
                    </div>
                </div>
                {/* Product info */}
                <div className="flex-1 space-y-4">
                    <div className="text-2xl font-bold text-blue-600 dark:text-blue-400">{formatPrice(product.data?.price??0)}</div>
                    {/* <div className="text-gray-700 dark:text-gray-200">{product.description}</div> */}
                    {/* <div className="text-sm text-gray-500 dark:text-gray-400">Mã sản phẩm: {product.sku}</div> */}
                    {/* <div className="text-sm text-gray-500 dark:text-gray-400">Danh mục: {product.category}</div> */}
                    {/* <div className="text-sm text-gray-500 dark:text-gray-400">Trạng thái: {product.isActive ? "Còn hàng" : "Hết hàng"}</div> */}
                    <div className="flex gap-4 mt-6">
                        <button
                            className="px-6 py-2 bg-blue-600 dark:bg-blue-500 text-white rounded-lg font-semibold hover:bg-blue-700 dark:hover:bg-blue-600 transition"
                            onClick={() => handleChooseColor("cart")}
                        >
                            Thêm vào giỏ hàng
                        </button>
                        <button
                            className="px-6 py-2 bg-red-600 dark:bg-red-500 text-white rounded-lg font-semibold hover:bg-red-700 dark:hover:bg-red-600 transition"
                            onClick={() => handleChooseColor("buy")}
                        >
                            Mua ngay
                        </button>
                    </div>
                </div>
            </div>

            {/* Modal chọn màu */}
            {showColorModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
                    <div className="bg-white dark:bg-neutral-900 rounded-lg p-6 min-w-[320px] shadow-lg">
                        <h2 className="text-lg font-bold mb-4 text-gray-900 dark:text-gray-100">Chọn loại và số lượng</h2>
                        <div className="flex gap-4 mb-6">
                            {COLOR_OPTIONS.map((color) => (
                                <button
                                    key={color.value}
                                    className={`w-10 h-10 rounded-full border-2 flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all ${color.class} ${selectedColor === color.value ? "ring-4 ring-blue-400" : ""}`}
                                    onClick={() => setSelectedColor(color.value)}
                                    aria-label={color.name}
                                >
                                    {color.value === "white" && <span className="block w-5 h-5 rounded-full bg-white border" />}
                                </button>
                            ))}
                        </div>
                        <div className="mb-6 flex items-center gap-3">
                            <label className="text-gray-700 dark:text-gray-200 font-medium">Số lượng:</label>
                            <input
                                type="number"
                                min={1}
                                value={quantity}
                                onChange={e => setQuantity(Math.max(1, Number(e.target.value)))}
                                className="w-20 px-2 py-1 rounded border border-gray-300 dark:border-neutral-700 bg-white dark:bg-neutral-800 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>
                        <div className="flex justify-end gap-2">
                            <button
                                className="px-4 py-2 rounded bg-gray-200 dark:bg-neutral-700 text-gray-700 dark:text-gray-100 hover:bg-gray-300 dark:hover:bg-neutral-600"
                                onClick={() => setShowColorModal(false)}
                            >
                                Huỷ
                            </button>
                            <button
                                className="px-4 py-2 rounded bg-blue-600 dark:bg-blue-500 text-white font-semibold hover:bg-blue-700 dark:hover:bg-blue-600 disabled:opacity-50"
                                onClick={handleConfirm}
                                disabled={!selectedColor || quantity < 1}
                            >
                                Xác nhận
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CustomerProductDetail;
