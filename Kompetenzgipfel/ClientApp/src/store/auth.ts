import {defineStore} from 'pinia';

export const useAuthStore = defineStore({
    id: 'auth',
    state: () => ({
        token: null as string | null,
    }),
    actions: {
        login(token: string) {
            this.token = token;
        },
        logout() {
            this.token = null;
        },
    },
});