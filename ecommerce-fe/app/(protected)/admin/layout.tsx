"use client";

import AdminHeader from "@/features/common/AdminHeader";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function AdminProtectedLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const [authorized, setAuthorized] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) router.replace("/login");
    else setAuthorized(true);
  }, [router]);

  if (!authorized)
    return (
      <div className="flex items-center justify-center h-screen text-gray-500">
        Đang kiểm tra quyền truy cập...
      </div>
    );

  return (
    <div className="flex min-h-screen bg-background">
      <div className="flex-1 flex flex-col">
        <AdminHeader />
        <main className="flex-1 p-5">{children}</main>
      </div>
    </div>
  );
}
