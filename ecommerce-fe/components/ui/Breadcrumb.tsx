"use client";
import Link from "next/link";
import { ChevronRight } from "lucide-react";

interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
}

export function Breadcrumb({ items }: BreadcrumbProps) {
  return (
    <nav className="flex items-center text-sm text-gray-500 dark:text-gray-400">
      {items.map((item, i) => {
        const isLast = i === items.length - 1;
        return (
          <div key={i} className="flex items-center">
            {item.href && !isLast ? (
              <Link
                href={item.href}
                className="hover:text-blue-600 dark:hover:text-blue-400 transition"
              >
                {item.label}
              </Link>
            ) : (
              <span className="text-gray-700 dark:text-gray-200 font-medium">
                {item.label}
              </span>
            )}
            {!isLast && (
              <ChevronRight
                size={16}
                className="mx-2 text-gray-400 dark:text-gray-500"
              />
            )}
          </div>
        );
      })}
    </nav>
  );
}
