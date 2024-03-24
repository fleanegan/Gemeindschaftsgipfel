import './assets/main.css'
import {createApp} from 'vue';
import {createPinia} from 'pinia';
import axios from 'axios';
import App from './App.vue';
import {useAuthStore} from './store/auth';
import router from './router'

const pinia = createPinia();

axios.interceptors.request.use(
    (config) => {
        const token = useAuthStore().token;
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

axios.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        const isComingFromLogin = error.request.responseURL.includes("auth/login");
        if (error.response && error.response.status === 401 && !isComingFromLogin) {
            router.push('/login');
        } else
            return Promise.reject(error);
    }
);

createApp(App).use(pinia).use(router).mount('#app');