"use client";

import * as RadixSwitch from "@radix-ui/react-switch";

export function Switch(props: React.ComponentPropsWithoutRef<typeof RadixSwitch.Root>) {
  return (
    <RadixSwitch.Root
      {...props}
      className="w-10 h-6 rounded-full relative transition-colors
                 bg-gray-300 data-[state=checked]:bg-gray-800
                 dark:bg-gray-700 dark:data-[state=checked]:bg-gray-200"
    >
      <RadixSwitch.Thumb
        className="block w-4 h-4 rounded-full shadow transform transition-transform
                   bg-gray-100 dark:bg-gray-800
                   translate-x-1 data-[state=checked]:translate-x-5"
      />
    </RadixSwitch.Root>
  );
}
