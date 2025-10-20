import axios from "axios";

// === Lấy token từ localStorage ===
const getToken = () => {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("token");
};

// === Khởi tạo Axios instance ===
const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:8000",
  headers: { "Content-Type": "application/json" },
  withCredentials: false, // true nếu BE dùng cookie
});

// === Interceptor: Gắn token vào mỗi request ===
api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// === Interceptor: Bắt lỗi 401 → logout ===
api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      // Xử lý logout hoặc refresh token
      console.warn("Unauthorized → logout...");
      if (typeof window !== "undefined") {
        localStorage.removeItem("token");
        window.location.href = "/login";
      }
    }
    return Promise.reject(err);
  }
);

export default api;
