<script lang="ts">
  import i18n from "lib/i18n";
  import { apiModules } from "lib/api";
  import type { Writable } from "svelte/store";

  export let activeModule: Writable<string>;

  let openCategories = new Set();

  function toggleCategory(categoryName: string) {
    if (openCategories.has(categoryName)) {
      openCategories.delete(categoryName);
    } else {
      openCategories.add(categoryName);
    }
    openCategories = openCategories;
  }

  function scrollToModule(event: MouseEvent, moduleName: string) {
    event.preventDefault();
    const section = document.getElementById(moduleName);
    if (section) {
      section.scrollIntoView({ behavior: "smooth" });
    }
  }
</script>

<nav id="sidebar">
  <h2>{$i18n.t("ModulesTitle")}</h2>
  <ul id="module-list">
    {#each apiModules as module}
      <li
        class:category={module.subcategory}
        class:open={openCategories.has(module.name)}
      >
        <a
          href="#{module.name}"
          class:active={$activeModule === module.name}
          on:click={(e) => {
            if (module.subcategory) {
              toggleCategory(module.name);
            }
            scrollToModule(e, module.name);
          }}
        >
          {module.name}
        </a>
        {#if module.subcategory}
          <ul class="sub-module-list">
            {#each module.subcategory as subModule}
              <li>
                <a
                  href="#{subModule.name}"
                  class:active={$activeModule === subModule.name}
                  on:click={(e) => scrollToModule(e, subModule.name)}
                  >{subModule.name}</a
                >
              </li>
            {/each}
          </ul>
        {/if}
      </li>
    {/each}
  </ul>
</nav>

<style lang="scss">
  #sidebar {
    width: 280px;
    flex-shrink: 0;
    background-color: var(--surface-color);
    border-right: 1px solid var(--border-color);
    overflow-y: auto;
    padding: 1rem;

    h2 {
      font-size: 1.1rem;
      padding-left: 0.5rem;
      margin-top: 0;
      margin-bottom: 0.5rem;
      color: var(--text-color-light);
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    #module-list {
      list-style: none;
      padding: 0;
      margin: 0;

      li {
        padding: 0;
        /* Override default li padding */
        border-bottom: none;
        /* Override default li border */
        margin-bottom: 0.25rem;

        a {
          display: block;
          padding: 0.5rem 0.75rem;
          color: var(--text-color);
          text-decoration: none;
          border-radius: 4px;
          transition:
            background-color 0.2s,
            color 0.2s;

          &:hover {
            background-color: var(--surface-color-hover);
            color: var(--text-color);
            text-decoration: none;
          }

          &.active {
            background-color: var(--primary);
            color: #fff;
            font-weight: 500;

            &:hover {
              background-color: var(--primary-hover);
              color: #fff;
            }
          }
        }
      }
    }

    .sub-module-list {
      padding-left: 1rem;
      border-left: 1px solid var(--border-color);
      margin-top: 0.25rem;
      margin-bottom: 0.25rem;

      li a {
        padding-left: 0.5rem;
        font-size: 0.9rem;

        &.active {
          background-color: var(--primary);
          color: #fff;

          &:hover {
            background-color: var(--primary-hover);
            color: #fff;
          }
        }
      }
    }
  }
</style>
