"use client";
import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";

interface BackButtonProps {
  label?: string;
  fallbackHref?: string;
  className?: string;
}

export function BackButton({ 
  label = "Quay lại", 
  fallbackHref = "/", 
  className = "" 
}: BackButtonProps) {
  const router = useRouter();

  const handleBack = () => {
    if (window.history.length > 1) {
      router.back();
    } else {
      router.push(fallbackHref);
    }
  };

  return (
    <button
      onClick={handleBack}
      className={`group relative inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg font-normal text-gray-700 dark:text-gray-200 text-xs transition-all duration-300 overflow-hidden ${className}`}
    >
      {/* Gradient background on hover */}
      <div className="absolute inset-0 bg-gradient-to-r from-gray-100 via-gray-50 to-gray-100 dark:from-gray-800 dark:via-gray-850 dark:to-gray-800 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
      
      {/* Border gradient effect */}
      <div className="absolute inset-0 rounded-xl bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 opacity-0 group-hover:opacity-100 transition-opacity duration-300 blur-sm -z-10"></div>
      
      {/* Subtle border */}
      <div className="absolute inset-0 rounded-xl border border-gray-200 dark:border-gray-700 group-hover:border-gray-300 dark:group-hover:border-gray-600 transition-colors duration-300"></div>
      
      {/* Icon with animation */}
      <ArrowLeft 
        size={14} 
        className="relative z-10 group-hover:-translate-x-1 transition-transform duration-300" 
      />
      
      {/* Label */}
      <span className="relative z-10 text-xs">
        {label}
      </span>
      
      {/* Shine effect on hover */}
      <div className="absolute inset-0 -translate-x-full group-hover:translate-x-full transition-transform duration-700 bg-gradient-to-r from-transparent via-white/20 to-transparent skew-x-12"></div>
    </button>
  );
}