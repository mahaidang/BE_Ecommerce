"use client";

import { useQuery } from "@tanstack/react-query";
import api from "@/lib/api";

export default function Home() {
  const { data, isLoading } = useQuery({
    queryKey: ["health"],
    queryFn: async () => (await api.get("/health")).data,
  });

  return (
    <main className="p-10">
      <h1 className="text-2xl font-bold">Ecommerce FE</h1>
      <p>{isLoading ? "Loading..." : `Server says: ${data}`}</p>
    </main>
  );
}
