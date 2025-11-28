<script lang="ts">
    import i18n from "lib/i18n";
    import { Dropdown } from "lib/language/dropdown";

    const languageOptions = [
        { value: "en", label: "English" },
        { value: "zh", label: "中文" },
    ];

    $: currentLanguage = $i18n.language;

    i18n.subscribe((store) => {
        currentLanguage = store.language;
    });

    function handleLanguageChange(event: CustomEvent<string>) {
        $i18n.changeLanguage(event.detail);
    }
</script>

<div class="language-switcher">
    <Dropdown
        options={languageOptions}
        value={currentLanguage}
        onchange={handleLanguageChange}
    />
</div>

<style lang="scss">
    .language-switcher {
        display: flex;
        justify-content: center;
        margin: 0 1.5rem;
    }
</style>
