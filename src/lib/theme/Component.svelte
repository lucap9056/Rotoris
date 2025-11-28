<script lang="ts">
	import { theme, toggleTheme } from "./";
	import { crossfade } from "svelte/transition";
	import { quintOut } from "svelte/easing";

	const [send, receive] = crossfade({
		duration: (d) => Math.sqrt(d * 200),

		fallback(node, params) {
			const style = getComputedStyle(node);
			const transform = style.transform === "none" ? "" : style.transform;

			return {
				duration: 200,
				easing: quintOut,
				css: (t) => `
					transform: ${transform} scale(${t});
					opacity: ${t}
				`,
			};
		},
	});
</script>

<button class="theme-toggle" on:click={toggleTheme}>
	{#if $theme === "dark"}
		<span in:send={{ key: "moon" }} out:receive={{ key: "sun" }}>
			<svg
				width="20"
				height="20"
				viewBox="0 0 24 24"
				fill="none"
				stroke="currentColor"
				stroke-width="2"
				stroke-linecap="round"
				stroke-linejoin="round"
			>
				<path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
			</svg>
		</span>
	{:else}
		<span in:send={{ key: "sun" }} out:receive={{ key: "moon" }}>
			<svg
				width="20"
				height="20"
				viewBox="0 0 24 24"
				fill="none"
				stroke="currentColor"
				stroke-width="2"
				stroke-linecap="round"
				stroke-linejoin="round"
			>
				<circle cx="12" cy="12" r="5" />
				<line x1="12" y1="1" x2="12" y2="3" />
				<line x1="12" y1="21" x2="12" y2="23" />
				<line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
				<line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
				<line x1="1" y1="12" x2="3" y2="12" />
				<line x1="21" y1="12" x2="23" y2="12" />
				<line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
				<line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
			</svg>
		</span>
	{/if}
</button>

<style lang="scss">
	.theme-toggle {
		height: 2rem;
		color: var(--color-text);
		background: none;
		border: none;
		padding: 0;
		margin: 0;
		cursor: pointer;

		& span {
			display: grid;
			place-content: center;
			transform-origin: center center;
			transition: transform 0.3s ease-in-out;
		}

		&:hover span {
			transform: scale(1.2) rotate(15deg);
		}
	}
</style>
