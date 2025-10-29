import React from "react";

const SellerProductList = () => {
  // TODO: Fetch and display only seller's own products
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Quản lý sản phẩm của bạn</h1>
      <button className="mb-4 px-4 py-2 bg-blue-600 text-white rounded">Thêm sản phẩm mới</button>
      <div className="border rounded p-4 bg-white shadow">
        {/* Danh sách sản phẩm của seller sẽ hiển thị ở đây */}
        <p>Chưa có sản phẩm nào.</p>
      </div>
    </div>
  );
};

export default SellerProductList;
