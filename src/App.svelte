<script lang="ts">
  import { theme } from "lib/theme";
  import { onMount } from "svelte";
  import Language from "lib/language/Component.svelte";
  import Theme from "lib/theme/Component.svelte";
  import Sidebar from "lib/Sidebar.svelte";
  import Content from "lib/Content.svelte";
  import Popover from "lib/popover/Component.svelte";
  import { writable } from "svelte/store";

  const activeModule = writable("");

  onMount(() => {
    theme.subscribe((value) => {
      document.body.classList.toggle("dark-theme", value === "dark");
    });
    activeModule.subscribe((moduleName) => {
      const newUrl = `${window.location.pathname}${window.location.search}${
        moduleName ? `#${moduleName}` : ""
      }`;
      history.replaceState(null, "", newUrl);
    });
  });
</script>

<header>
  <h1>Rotoris Lua API Documentation</h1>
  <Language />
  <Theme />
</header>
<main class="container">
  <Sidebar {activeModule} />
  <Content {activeModule} />
  <Popover />
</main>

<style lang="scss">
  header {
    flex-shrink: 0;
    padding: 1rem 1.5rem;
    border-bottom: 1px solid var(--border-color);
    background-color: var(--surface-color);
    display: flex;
    flex-direction: row;

    h1 {
      margin: 0;
      font-size: 1.25rem;
      flex: 1;
      display: flex;
      align-items: center;
    }
  }
  main.container {
    position: relative;
    display: flex;
    flex-direction: row;
    flex: 1;
    overflow-y: auto;
  }
</style>
