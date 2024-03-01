<template>
  <div class="login-container">
    <h1>Kennen wir uns?</h1>
    <div class="checkbox-container">
      <p :class="{active: !isSignup, inactive: isSignup}">Anmelden</p>
      <label class="switch"><input v-model="isSignup" type="checkbox" @click="errors=''"/>
        <div></div>
      </label>
      <p :class="{active: isSignup, inactive: !isSignup}">Neu hier ! </p>
    </div>
    <form class="login-form" @submit.prevent="submitData">
      <div class="form-group">
        <label for="username">{{ isSignup ? "Neuer Benutzername" : "Benutzername" }}</label>
        <input id="username" v-model="username" class="form-input" type="text"/>
      </div>
      <div class="form-group">
        <label for="password">{{ isSignup ? "Neues " : "" }}Passwort</label>
        <input id="password" v-model="password" class="form-input" type="password"/>
      </div>
      <div v-if="isSignup" class="form-group">
        <label for="passwordConfirmation">Passwort bestätigen</label>
        <input id="passwordConfirmation" v-model="passwordConfirmation" class="form-input" type="password"/>
      </div>
      <p v-if="isSignup && !passwordsMatching" class="errors">Die Passwörter stimmen nicht überein.</p>
      <div v-if="isSignup" class="form-group">
        <label for="entrySecret">Eintrittsgeheimnis</label>
        <input id="entrySecret" v-model="entrySecret" class="form-input" type="password"/>
      </div>
      <textarea v-if="errors!=''" class="errors" inputmode="none">{{ errors }}</textarea>
      <button class="submit-button" type="submit" @click="submitData">Abschicken</button>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import {useAuthStore} from '@/store/auth';
import axios, {AxiosError} from "axios";

interface ErrorResponseData {
  description: String
}

export default defineComponent({
      data() {
        return {
          username: '',
          password: '',
          passwordConfirmation: '',
          entrySecret: '',
          errors: '',
          isSignup: false,
        };
      },
      computed: {
        passwordsMatching: function passwordsMatching() {
          return this.password == this.passwordConfirmation;
        }
      },
      methods: {
        async submitData() {
          if (this.isSignup)
            await this.handleSignup()
          else
            await this.handleLogin()
        },
        async handleLogin() {
          try {
            console.log("handling login")
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
          } catch (e) {
            const r = e as AxiosError;
            if (r.response?.status == 401)
              this.errors = "Falsches Passwort oder nicht existierender Nutzername"
          }
        }
        , async handleSignup() {
          try {
            console.log("handling signup")
            const response = await axios.post('/api/auth/signup', {
              "UserName": this.username,
              "Password": this.password,
              "EntrySecret": this.entrySecret
            });

            if (response.status >= 200 && response.status < 300) {
              await this.handleLogin()
            }
          } catch (e: any) {
            var r = e as AxiosError;
            console.log(r)
            var responseData = r.response?.data as [ErrorResponseData];
            this.errors = responseData.map(object => object["description"] as string).join("\n")
          }
        }
      },
    },
)
</script>

<style scoped>

.checkbox-container {
  text-align: center;
  display: flex;
  place-content: center;
  align-content: center;
  align-items: center;
  flex-direction: row;
  margin: auto auto 2rem;
}

.switch input {
  position: absolute;
  opacity: 0;
}

.switch {
  display: inline-block;
  font-size: 1rem;
  height: 1.4rem;
  width: 2.4rem;
  border-radius: 1rem;
  border-color: var(--color-main-text);
  border-style: solid;
}

.switch div {
  height: 1rem;
  width: 1rem;
  border-radius: 1rem;
  background-color: var(--color-main-text);
  -webkit-transition: all 300ms;
  -moz-transition: all 300ms;
  transition: all 300ms;
  margin-top: 0.0rem;
}

.switch input:checked + div {
  -webkit-transform: translate3d(100%, 0, 0);
  -moz-transform: translate3d(100%, 0, 0);
  transform: translate3d(100%, 0, 0);
}

.checkbox-container p {
  font-size: 0.85rem;
  width: 5rem;
  margin-right: 1rem;
}

.active {
  color: var(--color-main-text);
}

.inactive {
  color: #adb5bd;
}

.login-form {
  display: flex;
  flex-direction: column;
}

.form-group {
  display: flex;
  flex-direction: column;
  padding: 0.5rem 1rem 0.5rem 1rem;
}

.errors {
  color: red;
  margin-left: 1.5rem;
  margin-right: 1.5rem;
  font-size: 0.75rem;
  resize: none;
  border: none;
}

.submit-button {
  margin-left: auto;
  margin-right: 1rem;
}

</style>
