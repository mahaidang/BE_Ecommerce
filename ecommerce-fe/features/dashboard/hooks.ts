import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "./api";

export function useDashboard(from?: string, to?: string) {
  const revenueByDate = useQuery({
    queryKey: ["dashboard", "revenue-by-date", from, to],
    queryFn: () => dashboardApi.getRevenueByDate(from, to),
    enabled: !!from && !!to,
  });
  const revenueByPayment = useQuery({
    queryKey: ["dashboard", "revenue-by-payment", from, to],  
    queryFn: () => dashboardApi.getRevenueByPayment(from, to),
    enabled: !!from && !!to,
  });
  const orderStatus = useQuery({
    queryKey: ["dashboard", "order-status"],
    queryFn: dashboardApi.getOrderStatusCount,
  });
  const paymentSummary = useQuery({
    queryKey: ["dashboard", "payment-summary"],
    queryFn: dashboardApi.getPaymentSummary,
  });
  return { revenueByDate, revenueByPayment, orderStatus, paymentSummary };
}