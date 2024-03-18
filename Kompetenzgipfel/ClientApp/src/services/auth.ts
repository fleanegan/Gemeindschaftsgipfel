import axios from 'axios';
import {useAuthStore} from '@/store/auth';

const authStore = useAuthStore();

export async function login(credentials: { username: string; password: string }) {
    try {
        const response = await axios.post('/api/auth/login', credentials);
        const token = response.data;
        authStore.login(token, credentials.username);
        return {token};
    } catch (error) {
        throw error;
    }
}