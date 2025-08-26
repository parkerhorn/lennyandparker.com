<script>
	import { cn } from "$lib/utils.js";

	let {
		class: className = "",
		variant = "default",
		size = "default", 
		ref = $bindable(null),
		href = undefined,
		type = "button",
		disabled = false,
		children,
		...restProps
	} = $props();

	// Clean button variants for wedding theme using design tokens
	function getVariantClasses(variant) {
		const variants = {
			default: "bg-primary text-primary-foreground hover:bg-primary/90",
			secondary: "bg-secondary text-secondary-foreground hover:bg-secondary/80",
			outline: "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
			ghost: "hover:bg-accent hover:text-accent-foreground",
			link: "text-primary underline-offset-4 hover:underline",
			// Wedding-themed variants with consistent design language
			wedding: "bg-[#c5b3e8] text-[#2d1a42] border-2 border-[#b29dd6] hover:bg-[#b29dd6] shadow-[4px_4px_0_#b29dd6] hover:shadow-[2px_2px_0_#9d87c4] hover:translate-x-0.5 hover:translate-y-0.5 transition-all duration-200 font-semibold underline decoration-[#2d1a42] decoration-2 underline-offset-4",
			weddingSecondary: "bg-[#c8d49a] text-[#3a4520] border-2 border-[#b5c285] hover:bg-[#b5c285] shadow-[4px_4px_0_#b5c285] hover:shadow-[2px_2px_0_#a2b070] hover:translate-x-0.5 hover:translate-y-0.5 transition-all duration-200 font-semibold underline decoration-[#3a4520] decoration-2 underline-offset-4"
		};
		return variants[variant] || variants.default;
	}

	function getSizeClasses(size) {
		const sizes = {
			default: "h-10 px-4 py-2",
			sm: "h-9 px-3 py-2 text-sm",
			lg: "h-11 px-8 py-3 text-base",
			icon: "h-10 w-10"
		};
		return sizes[size] || sizes.default;
	}

	const baseClasses = "inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50";
	
	const buttonClasses = $derived(cn(
		baseClasses,
		getVariantClasses(variant),
		getSizeClasses(size),
		className
	));
</script>

{#if href}
	<a
		bind:this={ref}
		class={buttonClasses}
		{href}
		aria-disabled={disabled}
		role={disabled ? "link" : undefined}
		tabindex={disabled ? -1 : undefined}
		{...restProps}
	>
		{@render children?.()}
	</a>
{:else}
	<button
		bind:this={ref}
		class={buttonClasses}
		type={type}
		{disabled}
		{...restProps}
	>
		{@render children?.()}
	</button>
{/if}