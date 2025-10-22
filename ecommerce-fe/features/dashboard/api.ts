import api from "@/lib/api";

export const dashboardApi = {
  getRevenueByDate: async (from?: string, to?: string) => {
    const params: Record<string, string> = {};
    if (from) params.from = from;
    if (to) params.to = to;
    return (await api.get("api/report/revenue-by-date", { params })).data;
  },
  getRevenueByPayment: async (from?: string, to?: string) => {
    const params: Record<string, string> = {};
    if (from) params.from = from;
    if (to) params.to = to;
    return (await api.get("api/report/revenue-by-payment", {params})).data;
  },
  getOrderStatusCount: async () => (await api.get("api/report/order-status-count")).data,
  getPaymentSummary: async () => (await api.get("api/report/payment-summary")).data,
};
