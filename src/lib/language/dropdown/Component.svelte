<script lang="ts">
    import { fly } from "svelte/transition";

    export let onchange: (event: CustomEvent<string>) => void;
    export let options: { value: string; label: string }[] = [];
    export let value: string;

    let isOpen = false;

    function selectOption(e: MouseEvent) {
        const target = e.currentTarget as HTMLElement;
        value = target.dataset.value || "";
        if (value === "") return;
        isOpen = false;
        onchange?.(new CustomEvent("change", { detail: value }));
    }

    function handleKeydown(event: KeyboardEvent) {
        if (event.key === "Escape") {
            isOpen = false;
        }
    }

    $: selectedLabel = options.find((opt) => opt.value === value)?.label || "";
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="custom-select" onkeydown={handleKeydown}>
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <div
        class="select-trigger"
        onclick={() => (isOpen = !isOpen)}
        role="button"
        aria-haspopup="listbox"
        aria-expanded={isOpen}
        tabindex="0"
    >
        <span>{selectedLabel}</span>
        <div class="arrow" class:open={isOpen}></div>
    </div>
    {#if isOpen}
        <div
            class="options"
            role="listbox"
            transition:fly={{ y: -10, duration: 200 }}
        >
            {#each options as option}
                <!-- svelte-ignore a11y_click_events_have_key_events -->
                <div
                    class="option"
                    class:selected={option.value === value}
                    data-value={option.value}
                    onclick={selectOption}
                    role="option"
                    aria-selected={option.value === value}
                    tabindex="0"
                >
                    {option.label}
                </div>
            {/each}
        </div>
    {/if}
</div>

{#if isOpen}
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <!-- svelte-ignore a11y_no_static_element_interactions -->
    <div class="overlay" onclick={() => (isOpen = false)}></div>
{/if}

<style lang="scss">
    .custom-select {
        position: relative;
        width: 150px;
        font-size: 1rem;
    }

    .select-trigger {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0.5em 1em;
        background-color: var(--surface-color);
        border: 1px solid var(--border-color);
        border-radius: 0.25em;
        cursor: pointer;
        user-select: none;
        transition:
            border-color 0.2s,
            box-shadow 0.2s;
    }

    .select-trigger:hover,
    .select-trigger:focus {
        border-color: var(--primary);
        box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
        outline: none;
    }

    .arrow {
        width: 0;
        height: 0;
        border-left: 5px solid transparent;
        border-right: 5px solid transparent;
        border-top: 5px solid #333;
        transition: transform 0.2s ease-in-out;
    }

    .arrow.open {
        transform: rotate(180deg);
    }

    .options {
        position: absolute;
        top: calc(100% + 5px);
        left: 0;
        right: 0;
        background-color: var(--surface-color);
        border: 1px solid var(--border-color);
        border-radius: 0.25em;
        z-index: 10;
        max-height: 200px;
        overflow-y: auto;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    }

    .option {
        padding: 0.5em 1em;
        cursor: pointer;
        transition: background-color 0.2s;
    }

    .option:hover,
    .option.selected {
        background-color: var(--surface-color-hover);
    }

    .overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        z-index: 9;
    }
</style>
