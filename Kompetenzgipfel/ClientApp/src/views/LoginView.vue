<template>
  <div>
    <h2>Login</h2>
    <form @submit.prevent="handleLogin">
      <div>
        <label for="username">Username:</label>
        <input id="username" v-model="username" type="text"/>
      </div>
      <div>
        <label for="password">Password:</label>
        <input id="password" v-model="password" type="password"/>
      </div>
      <button type="submit">Login</button>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import {useAuthStore} from '@/store/auth';
import axios from "axios";

export default defineComponent({
      data() {
        return {
          username: '',
          password: '',
        };
      },
      methods: {
        async handleLogin() {
          const {username, password} = this;
          // Simulate API call
          try {
            const response = await axios.post('/api/auth/login', {
              "UserName": this.username,
              "Password": this.password
            });

            if (response.status >= 200 && response.status < 300) {
              const responseData = response.data
              useAuthStore().login(responseData);
              let routeToPush = {path: '/'};
              if (this.$route.query.redirect) {
                routeToPush = {path: this.$route.query.redirect as string};
              }
              this.$router.push(routeToPush);
            }
          } catch {
          }
        }
      },
    },
)
</script>

<style scoped>
/* Add your component styles here */
</style>
