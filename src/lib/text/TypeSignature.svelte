<script lang="ts">
    import { type ApiClass, apiModules } from "lib/api";
    import { popover } from "lib/popover";

    export let type: ApiClass | string;
    export let isArray: boolean =
        typeof type === "string" ? false : type.isArray;

    const module = (() => {
        if (typeof type === "string") {
            return undefined;
        }

        for (const module of apiModules) {
            if (module.type.name === type.name) {
                return module;
            }
            if (module.subcategory) {
                const subModule = module.subcategory.find(
                    (s) => s.type.name === type.name,
                );
                if (subModule) {
                    return subModule;
                }
            }
        }
    })();

    const classClick = (e: MouseEvent) => {
        e.preventDefault();
        const target = e.target as HTMLElement;
        const moduleName = target.dataset.name;
        if (!moduleName) {
            return;
        }
        const section = document.getElementById(moduleName);
        if (section) {
            section.scrollIntoView({ behavior: "smooth" });
        }
    };

    const secondaryClassClick = (e: MouseEvent) => {
        e.preventDefault();
        if (e.target && typeof type !== "string") {
            const name = type.name;
            popover.set({ name, type });
        }
    };
</script>

{#if typeof type === "string"}
    <span>{type}</span>{#if isArray}{"[]"}{/if}
{:else}
    {#if module}<a href="##" data-name={module.name} onclick={classClick}
            >{type.name}</a
        >{:else}
        <!-- svelte-ignore a11y_click_events_have_key_events -->
        <!-- svelte-ignore a11y_no_static_element_interactions -->
        <a href="##" onclick={secondaryClassClick}>{type.name}</a>
    {/if}{#if isArray}{"[]"}{/if}{#if type.generics}
        {"<"}{#each type.generics as generic, index}{#if index !== 0}
                {"|"}{/if}<svelte:self type={generic} />{/each}{">"}
    {/if}
{/if}
