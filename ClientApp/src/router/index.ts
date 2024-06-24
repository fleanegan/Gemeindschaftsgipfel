// router/index.ts
import {createRouter, createWebHistory} from 'vue-router';
import HomeView from '../views/HomeView.vue';
import {useAuthStore} from '@/store/auth';

const router = createRouter({
    history: createWebHistory("/"),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView,
            meta: {requiresAuth: true}
        },
        {
            path: '/about',
            name: 'about',
            component: () => import('../views/AboutView.vue'),
            meta: {requiresAuth: false}
        },
        {
            path: '/login',
            name: 'login',
            component: () => import('../views/LoginView.vue'),
            meta: {requiresAuth: false}
        }, {
            path: '/topic',
            name: 'Inhalte',
            component: () => import('../views/TopicView.vue'),
        },
        {
            path: '/schedule',
            name: 'Ablaufplan',
            component: () => import('../views/ScheduleView.vue'),
        },
        {
            path: '/supporttask',
            name: 'HelfendeHaende',
            component: () => import('../views/SupportTaskView.vue'),
        },
        {
            path: '/topic/add',
            name: 'Neues Thema hinzufügen',
            component: () => import('../views/InputTopicView.vue'),
            props: false
        },
        {
            path: '/topic/edit:topicId',
            name: 'Thema bearbeiten',
            component: () => import('../views/InputTopicView.vue'),
            props: true
        }
    ]
});

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore(); // Move this inside the beforeEach guard
    if (to.meta.requiresAuth !== false && !authStore.token) {
        next({
                path: '/login',
                query: {redirect: to.fullPath}
            }
        );
    } else {
        // Continue navigation
        next();
    }
});

export default router;
