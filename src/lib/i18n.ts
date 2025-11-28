import { createI18nStore } from "svelte-i18next";
import HttpBackend from 'i18next-http-backend';
import i18next from 'i18next';

i18next.use(HttpBackend)
  .init({
    lng: 'en',
    fallbackLng: 'en',
    ns: ['translation'],
    defaultNS: 'translation',
    backend: {
      loadPath: './locales/{{lng}}/{{ns}}.json',
    },
    interpolation: {
      escapeValue: false,
    },
  });

const i18n = createI18nStore(i18next);
export default i18n;