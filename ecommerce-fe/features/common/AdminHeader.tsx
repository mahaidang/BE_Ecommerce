"use client";
import Link from "next/link";

const AdminHeader = () => {
  return (
    <header className="w-full bg-white dark:bg-neutral-900 shadow px-4 py-3 flex items-center justify-between">
      <div className="flex items-center gap-6">
        <Link href="/admin/dashboard" className="text-xl font-bold text-blue-600 dark:text-blue-400">E-Shop Admin</Link>
        <Link href="/admin/products" className="text-gray-700 dark:text-gray-100 hover:text-blue-600 dark:hover:text-blue-400 font-medium">Quản lý sản phẩm</Link>
        <Link href="/admin/orders" className="text-gray-700 dark:text-gray-100 hover:text-blue-600 dark:hover:text-blue-400 font-medium">Quản lý đơn hàng</Link>
        <Link href="/admin/users" className="text-gray-700 dark:text-gray-100 hover:text-blue-600 dark:hover:text-blue-400 font-medium">Quản lý người dùng</Link>
      </div>
      <div>
        <Link href="/admin/profile" className="text-gray-700 dark:text-gray-100 hover:text-blue-600 dark:hover:text-blue-400 font-medium">Trang cá nhân</Link>
      </div>
    </header>
  );
};

export default AdminHeader;
