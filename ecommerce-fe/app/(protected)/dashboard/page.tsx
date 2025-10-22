"use client";

import { useState } from "react";
import { useDashboard } from "@/features/dashboard/hooks";
import { LineChart, Line, PieChart, Pie, Cell, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid, BarChart, Legend, Bar } from "recharts";
import { Card } from "@/components/ui/card";

export default function DashboardPage() {
  const today = new Date();
  const weekAgo = new Date();
  weekAgo.setDate(today.getDate() - 30);

  const [from, setFrom] = useState(weekAgo.toISOString().slice(0, 10));
  const [to, setTo] = useState(today.toISOString().slice(0, 10));

  const { revenueByDate, revenueByPayment, orderStatus, paymentSummary } = useDashboard(from, to);
  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#A020F0", "#FF6384"];

  if (revenueByDate.isLoading || revenueByPayment.isLoading || orderStatus.isLoading || paymentSummary.isLoading) {
    return <div className="p-8 text-gray-500">Đang tải dữ liệu...</div>;
  }

  return (
    <main className="p-4 md:p-8 space-y-8 bg-background min-h-screen text-foreground">
      <form className="flex flex-col md:flex-row items-center gap-4 mb-6 bg-card rounded-lg shadow p-4 w-full max-w-2xl mx-auto">
        <label className="flex flex-col text-sm font-medium text-muted-foreground">
          Từ ngày
          <input
            type="date"
            className="border border-input rounded px-2 py-1 mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
            value={from}
            max={to}
            onChange={e => setFrom(e.target.value)}
          />
        </label>
        <label className="flex flex-col text-sm font-medium text-muted-foreground">
          Đến ngày
          <input
            type="date"
            className="border border-input rounded px-2 py-1 mt-1 focus:outline-none focus:ring-2 focus:ring-primary"
            value={to}
            min={from}
            max={today.toISOString().slice(0, 10)}
            onChange={e => setTo(e.target.value)}
          />
        </label>
      </form>

      <h1 className="text-3xl font-bold text-center text-primary mb-2 tracking-tight">Báo cáo tổng quan</h1>

      <section className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div className="bg-card rounded-xl shadow p-6 flex flex-col">
          <div className="text-lg font-semibold text-primary mb-4">Doanh thu theo ngày</div>
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
        <div className="bg-card rounded-xl shadow p-6 flex flex-col">
          <div className="text-lg font-semibold text-primary mb-4">Thống kê thanh toán</div>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={revenueByPayment.data}
              margin={{ top: 10, right: 30, left: 20, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="paymentProvider" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="totalAmount" fill="#8884d8" name="Tổng tiền" />
              <Bar dataKey="transactionCount" fill="#82ca9d" name="Số giao dịch" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </section>

      <section className="bg-card rounded-xl shadow p-6">
        <div className="text-lg font-semibold text-primary mb-4">Tóm tắt sự kiện thanh toán</div>
        <ResponsiveContainer width="100%" height={220}>
          <BarChart data={paymentSummary.data}>
            <XAxis dataKey="eventType" />
            <YAxis />
            <Tooltip formatter={(v) => `${v} lần`} />
            <Legend />
            <Bar dataKey="eventCount" fill="#4f46e5" name="Số lần xảy ra" />
          </BarChart>
        </ResponsiveContainer>
      </section>

      <section className="bg-card rounded-xl shadow p-6">
        <div className="text-lg font-semibold text-primary mb-4 text-center">Đơn hàng theo trạng thái</div>
        <div className="flex flex-col md:flex-row gap-8 justify-center items-center">
          <div className="flex-1 flex flex-col items-center">
            <div className="font-semibold mb-2 text-muted-foreground">Số lượng đơn</div>
            <ResponsiveContainer width={250} height={300}>
              <PieChart>
                <Pie
                  data={orderStatus.data}
                  dataKey="totalCount"
                  nameKey="status"
                  outerRadius={100}
                  labelLine={false}
                >
                  {orderStatus.data.map((entry: any, i: number) => (
                    <Cell key={i} fill={COLORS[i % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip formatter={(v: number) => `${v} đơn`} />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
          <div className="flex-1 flex flex-col items-center">
            <div className="font-semibold mb-2 text-muted-foreground">Tổng tiền</div>
            <ResponsiveContainer width={250} height={300}>
              <PieChart>
                <Pie
                  data={orderStatus.data}
                  dataKey="totalAmount"
                  nameKey="status"
                  outerRadius={100}
                  labelLine={false}
                >
                  {orderStatus.data.map((entry: any, i: number) => (
                    <Cell key={i} fill={COLORS[i % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip formatter={(v: number) => `${Number(v).toLocaleString("vi-VN")} đ`} />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>
      </section>
    </main>
  );
}
