"use client";
import * as RadixAlertDialog from "@radix-ui/react-alert-dialog";
import { cn } from "@/lib/utils";
import React from "react";

export const AlertDialog = RadixAlertDialog.Root;
export const AlertDialogTrigger = RadixAlertDialog.Trigger;
export const AlertDialogCancel = RadixAlertDialog.Cancel;
export const AlertDialogAction = RadixAlertDialog.Action;

export function AlertDialogContent({ children, className, ...props }: React.ComponentProps<typeof RadixAlertDialog.Content>) {
  return (
    <RadixAlertDialog.Portal>
      <RadixAlertDialog.Overlay className="fixed inset-0 z-40 bg-black/40 backdrop-blur-sm" />
      <RadixAlertDialog.Content
        className={cn(
          "fixed z-50 top-1/2 left-1/2 w-[95vw] max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg bg-background p-6 shadow-lg",
          className
        )}
        {...props}
      >
        {children}
      </RadixAlertDialog.Content>
    </RadixAlertDialog.Portal>
  );
}

export const AlertDialogHeader = ({ children }: { children: React.ReactNode }) => (
  <div className="mb-4">{children}</div>
);

export const AlertDialogTitle = RadixAlertDialog.Title;
export const AlertDialogDescription = RadixAlertDialog.Description;
export const AlertDialogFooter = ({ children }: { children: React.ReactNode }) => (
  <div className="flex justify-end gap-2 pt-2">{children}</div>
);
