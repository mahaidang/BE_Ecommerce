import React from "react";

const SellerProductForm = () => {
  // TODO: Form thêm/sửa sản phẩm cho seller
  return (
    <div>
      <h2 className="text-xl font-semibold mb-2">Thêm/Sửa sản phẩm</h2>
      <form className="space-y-4">
        {/* Các trường nhập liệu cho sản phẩm */}
        <input className="border p-2 rounded w-full" placeholder="Tên sản phẩm" />
        <input className="border p-2 rounded w-full" placeholder="Giá" type="number" />
        <textarea className="border p-2 rounded w-full" placeholder="Mô tả" />
        <button className="px-4 py-2 bg-green-600 text-white rounded">Lưu</button>
      </form>
    </div>
  );
};

export default SellerProductForm;
