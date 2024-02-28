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
        } else {
        }
        return config;
    },
    (error) => Promise.reject(error)
);

createApp(App).use(pinia).use(router).mount('#app');