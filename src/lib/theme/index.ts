import { writable } from 'svelte/store';
import cookie from "lib/cookie";

export const theme = writable<string>((() => {
    let savedTheme = cookie.get("theme");
    if (savedTheme) {
        return savedTheme;
    } else {
        const systemPrefersDark = window.matchMedia(
            "(prefers-color-scheme: dark)",
        ).matches;
        return systemPrefersDark ? "dark" : "light";
    }
})());

theme.subscribe((value) => {
    cookie.set("theme", value, 365);
});

window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", (e) => {
    if (!cookie.get("theme")) {
        theme.set(e.matches ? "dark" : "light");
        document.body.classList.toggle("dark-theme", e.matches);
    }
});

export function toggleTheme() {
    theme.update((value) => value === "dark" ? "light" : "dark");
}
