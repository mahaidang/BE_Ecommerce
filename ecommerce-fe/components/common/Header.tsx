"use client";


import { useTheme } from "next-themes";
import { Moon, Sun } from "lucide-react";
import { useAuth } from "@/features/auth/useAuth";

export default function Header() {
  // Header này chỉ dùng cho admin, đã có AdminHeader và CustomerHeader riêng cho từng role
  const { logout } = useAuth();
  const { theme, setTheme } = useTheme();
  return (
    <header className="flex items-center justify-between px-4 py-3 border-b bg-background sticky top-0 z-10">
      <h1 className="text-lg font-semibold">Ecommerce Admin</h1>
      <div className="flex items-center space-x-4">
        <button
          onClick={() => setTheme(theme === "light" ? "dark" : "light")}
          className="p-2 rounded hover:bg-muted"
        >
          {theme === "light" ? <Moon size={18} /> : <Sun size={18} />}
        </button>
        <button
          onClick={logout}
          className="text-sm text-red-500 hover:underline"
        >
          Logout
        </button>
      </div>  
    </header>
  );
}
