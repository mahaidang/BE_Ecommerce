import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "./api";

export function useDashboard(from?: string, to?: string) {
  const revenueByDate = useQuery({
    queryKey: ["dashboard", "revenue-by-date", from, to],
    queryFn: () => dashboardApi.getRevenueByDate(from, to),
    enabled: !!from && !!to,
  });

  return { revenueByDate };
}