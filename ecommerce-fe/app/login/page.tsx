"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useAuth } from "@/features/auth/useAuth";
import { useRouter } from "next/navigation";

const schema = z.object({
    username: z.string().min(2).max(100),
    password: z.string().min(6).max(100),
});

type FormData = z.infer<typeof schema>;

export default function LoginPage() {
  const { login } = useAuth();
  const router = useRouter();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({ resolver: zodResolver(schema) });

  const onSubmit = async (data: FormData) => {
    try {
      await login(data.username, data.password);
      router.push("/dashboard");
    } catch {
      alert("Đăng nhập thất bại. Kiểm tra lại tài khoản hoặc mật khẩu.");
    }
  };

  return (
    <main className="flex min-h-screen items-center justify-center bg-gray-50">
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="bg-white shadow-lg p-8 rounded-xl w-96 space-y-5"
      >
        <h1 className="text-2xl font-semibold text-center">Đăng nhập</h1>

        <div>
          <label className="block text-sm mb-1">Username hoặc Email</label>
          <input
            type="username"
            {...register("username")}
            className="w-full border rounded px-3 py-2"
          />
          {errors.username && (
            <p className="text-sm text-red-600">{errors.username.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm mb-1">Mật khẩu</label>
          <input
            type="password"
            {...register("password")}
            className="w-full border rounded px-3 py-2"
          />
          {errors.password && (
            <p className="text-sm text-red-600">{errors.password.message}</p>
          )}
        </div>

        <button
          disabled={isSubmitting}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded transition"
        >
          {isSubmitting ? "Đang đăng nhập..." : "Đăng nhập"}
        </button>
      </form>
    </main>
  );
}