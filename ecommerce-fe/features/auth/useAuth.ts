"use client";

import { useState, useEffect, use } from "react";
import api from "@/lib/api";

export function useAuth() {
    const [token, setToken] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
            const stored = localStorage.getItem("token");
            if (stored)setToken(stored);
            setLoading(false);
    }, []);

    const login = async (usernameOrEmail: string, password: string) => {
        const response = await api.post("api/identity/api/v1/auth/login", { usernameOrEmail, password });
        const accessToken = response.data?.token;
        if (accessToken) {
            localStorage.setItem("token", accessToken);
            setToken(accessToken);
        }
        return accessToken;
    }

    const logout = () => {
        localStorage.removeItem("token");
        setToken(null);
        window.location.href = "/login";
    }
    
    const refresh = async () => {
    console.log("refresh token (stub)");

    };
    return { token, loading, login, logout, refresh, isAuthenticated: !!token };
}