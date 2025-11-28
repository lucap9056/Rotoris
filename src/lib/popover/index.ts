import type { ApiModule } from 'lib/api';
import { writable } from 'svelte/store';

export const popover = writable<ApiModule | null>(null);