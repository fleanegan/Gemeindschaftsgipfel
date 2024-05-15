import './assets/main.css'
import {createApp} from 'vue';
import {createPinia} from 'pinia';
import axios from 'axios';
import App from './App.vue';
import {useAuthStore} from './store/auth';
import {useDataStore} from './store/data';
import router from './router'
import GlobalLoading from './views/GlobalLoadingView.vue';

const pinia = createPinia();

axios.interceptors.request.use(
    (config) => {
        const token = useAuthStore().token;
	useDataStore().startLoading();
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
	useDataStore().stopLoading();
        return Promise.reject(error);
    }
);

axios.interceptors.response.use(
    async (response) => {
	useDataStore().stopLoading();
        return response;
    },
    (error) => {
	useDataStore().stopLoading();
        const isComingFromLogin = error.request.responseURL.includes("auth/login");
        if (error.response && error.response.status === 401 && !isComingFromLogin) {
            router.push('/login');
        } else
            return Promise.reject(error);
    }
);

const app = createApp(App).use(pinia).use(router);

app.component('global-loading', GlobalLoading);

app.mount('#app');
