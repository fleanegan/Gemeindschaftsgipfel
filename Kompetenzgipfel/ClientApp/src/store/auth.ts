import {defineStore} from 'pinia';

export const useAuthStore = defineStore({
    id: 'auth',
    state: () => ({
        token: null as string | null,
        userName: null as string | null,
    }),
    actions: {
        login(token: string, userName: string) {
            this.token = token;
            this.userName = userName;
        },
        logout() {
            this.token = null;
            this.userName = null;
        }
    },
});