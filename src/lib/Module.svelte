<script lang="ts">
  import {
    isMethod,
    type ApiMethod,
    type ApiModule,
    type ApiProperty,
  } from "lib/api";
  import i18n from "lib/i18n";
  import ClassMember from "lib/text/ClassMember.svelte";
  import TypeSignature from "lib/text/TypeSignature.svelte";
  export let module: ApiModule;

  $: members = module.type.members;

  $: properties = (() => {
    if (!members) return [];

    const props: { name: string; details: ApiProperty | ApiProperty[] }[] = [];
    for (const memberName in members) {
      if (!isMethod(members[memberName])) {
        props.push({
          name: memberName,
          details: members[memberName] as ApiProperty | ApiProperty[],
        });
      }
    }
    return props;
  })();

  $: methods = (() => {
    if (!members) return [];
    const meths: { name: string; details: ApiMethod }[] = [];
    for (const memberName in members) {
      if (isMethod(members[memberName])) {
        const memberDetails = members[memberName];

        meths.push({ name: memberName, details: memberDetails });
      }
    }
    return meths;
  })();

</script>

<div id={module.name} class="module-section">
  <h2>
    {module.name}{#if module.type.generics}
      {"<"}{#each module.type.generics as generic, index}
        {#if index !== 0}
          {"|"}
        {/if}<TypeSignature type={generic} />
      {/each}{">"}
    {/if}
  </h2>
  <span>{$i18n.t(module.type.name + ".Description")}</span>
  {#if members === null || (properties.length === 0 && methods.length === 0)}
    <p>{$i18n.t("NoDocumented")}</p>
  {:else}
    <div class="module-content">
      {#if properties.length > 0}
        <h3>Properties</h3>
        <ul class="members-list">
          {#each properties as { name, details }}
            <ClassMember className={module.type.name} {name} {details} />
          {/each}
        </ul>
      {/if}

      {#if methods.length > 0}
        <h3>Methods</h3>
        <ul class="members-list">
          {#each methods as { name, details }}
            <ClassMember className={module.type.name} {name} {details} />
          {/each}
        </ul>
      {/if}
      {#if module.type.example}
        <h3>Example</h3>
        <pre class="language-lua"><code>{module.type.example.trim()}</code></pre>
        <br />
      {/if}
    </div>
  {/if}
</div>

<style lang="scss">
  h2 {
    font-size: 1.75rem;
    border-bottom: 1px solid var(--border-color);
    padding-bottom: 0.5rem;
    margin-bottom: 1rem;
    margin-top: 1.5rem;
  }

  h3 {
    font-size: 1.4rem;
    margin-top: 0;
    margin-bottom: 1rem;
  }

  .module-content {
    padding: 0 2rem;
    background-color: var(--surface-color);
    margin-top: 1rem;
    padding-top: 0.5rem;
    border-radius: 5px;
  }
</style>
