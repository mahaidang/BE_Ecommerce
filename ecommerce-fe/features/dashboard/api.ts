import api from "@/lib/api";

export const dashboardApi = {
  getRevenueByDate: async () => (await api.get("api/report/dashboard/revenue-by-date")).data,
  getPaymentDistribution: async () => (await api.get("api/report/dashboard/payment-distribution")).data,
};
