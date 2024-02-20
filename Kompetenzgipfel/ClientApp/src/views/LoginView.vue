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
      <div>
        <label for="result">Result:</label>
        <textarea id="result" v-model="result" type="text"/>
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
          result: ''
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
              this.result = responseData
              useAuthStore().login(responseData);
              // Redirect or perform any other action after successful login
              console.log('Logged in successfully!');
            }
          } catch {
            this.result = 'fail'
            console.log('Failed!');
          }
        }
      },
    },
)
</script>

<style scoped>
/* Add your component styles here */
</style>
