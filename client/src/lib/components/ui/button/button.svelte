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
			wedding: "bg-purple-200 text-purple-900 border-2 border-purple-300 hover:bg-purple-300 shadow-[4px_4px_0_theme(colors.purple.300)] hover:shadow-[2px_2px_0_theme(colors.purple.300)] hover:translate-x-0.5 hover:translate-y-0.5 transition-all duration-200 font-semibold underline decoration-purple-400 decoration-2 underline-offset-4",
			weddingSecondary: "bg-green-200 text-green-900 border-2 border-green-300 hover:bg-green-300 shadow-[4px_4px_0_theme(colors.green.300)] hover:shadow-[2px_2px_0_theme(colors.green.300)] hover:translate-x-0.5 hover:translate-y-0.5 transition-all duration-200 font-semibold underline decoration-green-400 decoration-2 underline-offset-4"
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