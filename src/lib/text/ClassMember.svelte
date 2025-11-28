<script lang="ts">
    import { type ApiMethod, type ApiProperty, isMethod } from "lib/api";
    import TypeSignature from "lib/text/TypeSignature.svelte";
    import i18n from "lib/i18n";

    export let className: string;
    export let name: string;
    export let details: ApiMethod | ApiProperty | ApiProperty[];
    undefined;
</script>

<li>
    <strong class="member-name"
        >{name.replace(
            /\./g,
            "",
        )}{#if isMethod(details) && details.isOptional}?{/if}</strong
    >
    {#if isMethod(details)}
        ({#if details.parameters}
            {#each details.parameters as param, i}
                {#if i !== 0}
                    {", "}
                {/if}
                <span class="param">
                    {param.name}{param.isOptional ? "?" : ""}:
                    {#each param.types as type, index}
                        {#if index !== 0}
                            {" |"}
                        {/if}
                        <TypeSignature {type} />
                    {/each}
                </span>
            {/each}
        {/if})
        {#if details.return}
            {":"}
            {#each details.return.types as type, index}
                {#if index !== 0}
                    {"|"}
                {/if}
                <TypeSignature {type} />
            {/each}
        {/if}
    {:else}
        {":"}
        {#each Array.isArray(details) ? details : [details] as { type, isArray }, index}
            {#if index !== 0}
                {"|"}
            {/if}
            <TypeSignature {type} {isArray} />
        {/each}
    {/if}
    <br />
    <span>{$i18n.t(`${className}.${name}`)}</span>
</li>
<br />
