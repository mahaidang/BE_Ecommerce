"use client";


import { useState } from "react";
import { useDashboard } from "@/features/dashboard/hooks";
import { LineChart, Line, PieChart, Pie, Cell, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from "recharts";
import { Card } from "@/components/ui/card";

export default function DashboardPage() {
  const today = new Date();
  const weekAgo = new Date();
  weekAgo.setDate(today.getDate() - 30);

  const [from, setFrom] = useState(weekAgo.toISOString().slice(0, 10));
  const [to, setTo] = useState(today.toISOString().slice(0, 10));

  const { revenueByDate } = useDashboard(from, to);

  if (revenueByDate.isLoading) {
    return <div className="p-8 text-gray-500">Đang tải dữ liệu...</div>;
  }

  return (
    <main className="p-6 space-y-6">
      <h1 className="text-2xl font-semibold mb-2">Doanh thu</h1>

      <form className="flex items-center gap-4 mb-4">
        <label className="flex flex-col text-sm">
          Từ ngày
          <input
            type="date"
            className="border rounded px-2 py-1"
            value={from}
            max={to}
            onChange={e => setFrom(e.target.value)}
          />
        </label>
        <label className="flex flex-col text-sm">
          Đến ngày
          <input
            type="date"
            className="border rounded px-2 py-1"
            value={to}
            min={from}
            max={today.toISOString().slice(0, 10)}
            onChange={e => setTo(e.target.value)}
          />
        </label>
      </form>

      <div className="p-5 border border-gray-300 rounded-md">
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={revenueByDate.data}
            margin={{ top: 10, right: 30, left: 20, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" tickFormatter={(v) => new Date(v).toLocaleDateString("vi-VN")} />
            <YAxis />
            <Tooltip formatter={(v: number) => v.toLocaleString("vi-VN") + " đ"} />
            <Line type="monotone" dataKey="totalRevenue" stroke="#4f46e5" strokeWidth={2} dot />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </main>
  );
}
