<template>
  <header :class="{'nav_header': true, 'nav_header_sticky': isSticky && isStandardPage}">
    <nav class="nav-links">
      <router-link v-if="isStandardPage" class="router-link" to="/"><img alt="Home" src="/icon.svg"
                                                                         style="width: 6rem; height: 6rem; max-height: 48px; max-width: 48px;">
      </router-link>
      <div class="transparent-header-area"></div>
      <router-link v-if="isStandardPage" class="router-link" to="/about">About</router-link>
      <router-link v-if="isStandardPage" class="router-link" to="/login">+</router-link>
      <router-link class="router-link" to="/topic">Vortragsthemen</router-link>
    </nav>
  </header>
  <div :class="{'routed-elements': isStandardPage, 'home_page_routed_elements': !isStandardPage}">
    <router-view/>
  </div>
</template>

<script lang="ts">
import {defineComponent, watch} from 'vue';
import {useRoute} from "vue-router";

export default defineComponent({
  data() {
    return {
      isSticky: false,
      isStandardPage: true
    };
  },
  methods: {
    handleScroll: function () {
      this.isSticky = window.scrollY > 0;
    },
    updateHeader: function () {
      console.log("manamana dip di bi dibi")
      const route = useRoute();
    },
  },
  mounted() {
    window.addEventListener('scroll', this.handleScroll);

    const currentRoute = useRoute();
    watch(currentRoute, (to, from) => {
      this.isStandardPage = to.path !== '/';
    });
  },
  beforeUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
  },
});
</script>

<style scoped src="./assets/header.css"></style>
