"use client";

import * as RadixDialog from "@radix-ui/react-dialog";
import { cn } from "@/lib/utils";
import React from "react";

export const Dialog = RadixDialog.Root;
export const DialogTrigger = RadixDialog.Trigger;
export const DialogClose = RadixDialog.Close;

export function DialogContent({ children, className, ...props }: React.ComponentProps<typeof RadixDialog.Content>) {
	return (
		<RadixDialog.Portal>
			<RadixDialog.Overlay className="fixed inset-0 z-40 bg-black/40 backdrop-blur-sm" />
			<RadixDialog.Content
				className={cn(
					"fixed z-50 top-1/2 left-1/2 w-[95vw] max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg bg-background p-6 shadow-lg",
					className
				)}
				{...props}
			>
				{children}
			</RadixDialog.Content>
		</RadixDialog.Portal>
	);
}

export const DialogHeader = ({ children }: { children: React.ReactNode }) => (
	<div className="mb-4">{children}</div>
);

export const DialogTitle = RadixDialog.Title;
