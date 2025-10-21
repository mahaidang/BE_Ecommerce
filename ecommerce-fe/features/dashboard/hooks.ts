import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "./api";

export function useDashboard() {
  const summary = useQuery({
    queryKey: ["dashboard", "summary"],
    queryFn: dashboardApi.getSummary,
  });

  const revenue = useQuery({
    queryKey: ["dashboard", "revenue"],
    queryFn: dashboardApi.getRevenueByDate,
  });

  const payments = useQuery({
    queryKey: ["dashboard", "payments"],
    queryFn: dashboardApi.getPaymentDistribution,
  });

  return { summary, revenue, payments };
}
