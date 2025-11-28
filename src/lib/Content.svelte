<script lang="ts">
  import { apiModules } from "lib/api";
  import Module from "./Module.svelte";
  import type { Writable } from "svelte/store";
  import { onMount } from "svelte";

  export let activeModule: Writable<string>;

  let sections: HTMLElement[] = [];
  let contentEl: HTMLElement;

  function updateActiveLink() {
    if (!contentEl) return;
    const scrollPosition = contentEl.scrollTop;
    const offset = 80;
    let current = "";

    for (const section of sections) {
      if (section.offsetTop <= scrollPosition + offset) {
        current = section.getAttribute("id") ?? "";
      }
    }
    activeModule.set(current);
  }

  onMount(() => {
    sections = Array.from(contentEl.querySelectorAll(".module-section"));
  });
</script>

<main id="content" bind:this={contentEl} on:scroll={updateActiveLink}>
  {#each apiModules as module}
    <Module {module} />
    {#if module.subcategory}
      {#each module.subcategory as sub}
        <Module module={sub} />
      {/each}
    {/if}
  {/each}
</main>

<style lang="scss">
  #content {
    padding: 1.5rem 2rem;
    overflow-y: auto;
    line-height: 1.6;
    flex: 1;
    padding-bottom: 50vh;
  }
</style>
