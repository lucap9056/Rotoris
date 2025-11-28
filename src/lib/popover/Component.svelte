<script lang="ts">
  import { popover } from "./";
  import Module from "lib/Module.svelte";
  import { setContext } from "svelte";

  setContext("popover-context", { isInsidePopover: true });

  let popoverTimeout: number;

  function handleMouseOver() {
    clearTimeout(popoverTimeout);
  }

  function handleMouseOut() {
    popoverTimeout = setTimeout(() => {
      popover.set(null);
    }, 300);
  }

  function handleClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const link: HTMLElement | null = target.closest("a.secondary-class");
    if (link) {
      event.preventDefault();
      const className = link.dataset.className;
      if (className) {
        popover.update((p) => (p ? { ...p, className } : null));
      }
    }
  }
</script>

{#if $popover}
  <!-- svelte-ignore a11y_click_events_have_key_events -->
  <!-- svelte-ignore a11y_no_static_element_interactions -->
  <!-- svelte-ignore a11y_mouse_events_have_key_events -->
  <div
    class="class-popover"
    style="
    display: block;"
    on:mouseover={handleMouseOver}
    on:mouseout={handleMouseOut}
    on:click={handleClick}
  >
    <Module module={$popover} />
  </div>
{/if}

<style lang="scss">
  .class-popover {
    background-color: var(--background);
    padding: 0 1rem;
    border: solid 2px var(--border-color);
    border-radius: 7px;
    display: none;
    position: fixed;
    width: auto;
    height: auto;
    overflow: auto;

    position: absolute;
    top: 1rem;
    right: 1rem;
    max-height: calc(100% - 2rem);
    max-width: 50vw;
  }
</style>
